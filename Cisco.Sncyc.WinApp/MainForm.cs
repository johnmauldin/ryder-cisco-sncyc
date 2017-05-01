using Accellos.Business.Contracts;
using Accellos.Business.Entities;
using Accellos.Data.Contracts;
using Cisco.Sncyc.Business.BusinessEngines;
using Core.Common.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cisco.Sncyc.WinApp
{
    public partial class MainForm : Form
    {
        [Import]
        ICiscoSnCycEngine _engine = null;

        string _serialNo;
        string _opcode;
        int _qty;
        MCustH _customer;
        MLoc _location;
        MItemH _item;
        string _bulkItemFlag = "N";
        EntryMode _entryMode;

        enum EntryMode
        {
            Customer,
            Location,
            Product,
            ProductError,
            BulkItem,
            ItemType,
            SerialNo,
            AdminRescan,
            Finished
        }

        public MainForm(string opcode)
        {
            InitializeComponent();
            ObjectBase.Container.SatisfyImportsOnce(this);
            _opcode = opcode;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            
            // this Archive could be split out to a job/scheduled task if neessary
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                _engine.ArchiveSerials();
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                showError(ex.Message);
            }
            

            showCustomerPrompt();
        }

        #region Properties
        MCustH SelectedCustomer
        {
            get { return _customer; }
            set 
            { 
                _customer = value;
                
                if (_engine.IsCustomerValid(_customer))
                {
                    hideError();
                    lblCustomer.Text = _customer.CustCode;
                    lblCustomer.Visible = true;
                    showLocationPrompt();
                }
                else
                {
                    showError("Invalid Customer Code");
                    lblCustomer.Visible = false;
                    txtScan.SelectAll();
                    _customer = null;
                }
            }
        }

        MLoc SelectedLocation
        {
            get { return _location; }
            set
            {
                _location = value;

                if (_engine.IsLocationValid(_location))
                {
                    if (!_engine.IsLocationCounted(_location.LocCode))
                    {
                        hideError();
                        lblLocation.Text = _location.LocCode;
                        lblLocation.Visible = true;
                        showProductPrompt();
                    }
                    else
                    {
                        showError("Location has been counted");
                        txtScan.SelectAll();
                        lblLocation.Visible = false;
                        _location = null;
                    }
                }
                else
                {
                    showError(string.Format("Location '{0}' Invalid", txtScan.Text));
                    txtScan.SelectAll();
                    _location = null;
                }

                
            }
        }

        MItemH SelectedProduct { get { return _item; }
            set
            {
                _item = value;
                bool valid;

                try
                {
                    valid = _engine.IsProductValid(_item);
                }
                catch (Exception ex)
                {
                    showProductPrompt();
                    showError(ex.Message);
                    return;
                }
                
                if (valid)
                {
                    if (_item.Serialized)
                    {
                        hideError();
                        this.lblProduct.Text = _item.ItemCode;
                        lblProduct.Visible = true;
                        showProductDetails();

                        if (_entryMode == EntryMode.ProductError)
                        {
                            _entryMode = EntryMode.Product;
                            return; //cloc lookup likely failed
                        }

                        if (_entryMode != EntryMode.Finished && 
                            _entryMode != EntryMode.AdminRescan)     
                            showBulkItemPrompt();

                    }
                    else
                    {
                        showError("Not a serialized item");
                        txtScan.SelectAll();
                        lblProduct.Visible = false;
                        lblItemQty.Visible = false;
                        _item = null;
                    }
                    
                }
                else
                {
                    showError("Item Invalid");
                    txtScan.SelectAll();
                    lblProduct.Visible = false;
                    _item = null;
                }
            }
        }

        bool IsBulkItem { get { return this._bulkItemFlag.Equals("Y"); } }

        string BulkItemFlag
        {
            get { return _bulkItemFlag; }
            set 
            {
                _bulkItemFlag = value.ToUpper();

                if (_engine.IsBulkFlagValid(_bulkItemFlag))
                {
                    
                    hideError();
                    showSerialNoPrompt();
                    if (this.IsBulkItem)
                        refreshProductHeader(); 
                }
                else
                {
                    showError("Enter Y or N");
                    _bulkItemFlag = string.Empty;
                }
            }
        }

        string _itemType;
        string ItemType
        {
            get { return _itemType; }
            set
            {
                _itemType = value.ToUpper();
            }
        }

        string ScanValue { get { return txtScan.Text.ToUpper().TrimEnd(); } }
        #endregion

        private void txtScan_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                if (Back()) return;
                if (Bulk()) return;
                if (Rescan()) return;
            }

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                if (Back()) return base.ProcessCmdKey(ref msg, keyData);
                if (Bulk()) return base.ProcessCmdKey(ref msg, keyData);
                if (Rescan()) return base.ProcessCmdKey(ref msg, keyData);
                submit();
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        #region txtScan_TextChanged
        private void txtScan_TextChanged(object sender, EventArgs e)
        {
            
        }
        #endregion

        void submit()
        {
            if (ScanValue.Length == 0)
            {
                return;
            }

            if (ScanValue.Length > 1) //when we require keypress [Enter] this length check is N/A
            {
                switch (_entryMode)
                {
                    case EntryMode.ItemType:
                        if (_engine.IsValidItemType(this.ScanValue))
                        {
                            this.ItemType = this.ScanValue;
                            refreshProductHeader();

                            try
                            {
                                Cursor.Current = Cursors.WaitCursor;

                                _engine.AddSerialNo(_item, _location,
                                _serialNo, this.ItemType, this.BulkItemFlag, _opcode);

                                addedSerialNo();

                                Cursor.Current = Cursors.Default;
                            }
                            catch (Exception ex)
                            {
                                Cursor.Current = Cursors.Default;
                                showSerialNoPrompt();
                                showError(ex.Message);
                            }

                        }
                        else
                        {
                            showError("Invalid Item Type");
                            txtScan.SelectAll();
                        }
                        return;

                    case EntryMode.SerialNo:
                        if (_engine.IsValidSerialNo(ScanValue))
                        {
                            _serialNo = this.ScanValue;

                            try
                            {
                                Cursor.Current = Cursors.WaitCursor;

                                var exists = _engine.SerialNoExists(this.SelectedCustomer.CustCode,
                                    this.SelectedLocation.LocCode,
                                    this.SelectedProduct.ItemCode,
                                    _serialNo);

                                if (exists)
                                {
                                    throw new InvalidOperationException(string.Format("{0} already scanned", _serialNo));
                                }
                                hideError();
                                showItemTypePrompt();
                            }
                            catch (Exception ex)
                            {
                                showSerialNoPrompt();
                                showError(ex.Message);
                            }
                            finally
                            {
                                Cursor.Current = Cursors.Default;
                            }
                        }
                        else
                        {
                            _serialNo = string.Empty;
                            showError("Not a valid serialno");
                            txtScan.SelectAll();
                        }
                        return;

                }

                try
                {
                    Cursor.Current = Cursors.WaitCursor;

                    if (this.SelectedCustomer == null)
                    {
                        this.SelectedCustomer = _engine.GetCustomer(ScanValue);
                        return;
                    }

                    if (this.SelectedLocation == null)
                    {
                        this.SelectedLocation = _engine.GetLocation(this.SelectedCustomer.CompCode, ScanValue);
                        return;
                    }

                    if (this.SelectedProduct == null)
                    {
                        this.SelectedProduct = _engine.GetProduct(this.SelectedCustomer.CompCode, this.SelectedCustomer.CustCode, ScanValue);
                        return;
                    }


                }
                catch (Exception ex)
                {
                    showError(ex.Message);
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        #region show

        void showCustomerPrompt()
        {
            _location = null;
            _customer = null;
            setPromptWidth();
            pnlScan.Visible = true;
            txtScan.Focus();
            txtScan.Clear();
            lblScan.Text = "Customer >>";
            lblCustomer.Visible = false;
            _entryMode = EntryMode.Customer;
        }

        void showLocationPrompt()
        {
            setPromptWidth();
            txtScan.Clear();
            lblScan.Text = "Location >>";
            lblLocation.Visible = false;
            lblItemQty.Visible = false;
            _location = null;
            _item = null;
            _itemType = string.Empty;
            _bulkItemFlag = string.Empty;
            _entryMode = EntryMode.Location;
        }

        void showProductPrompt()
        {
            _item = null; //reset the variable so next scan will take
            _qty = 0;
            _itemType = string.Empty;
            _bulkItemFlag = string.Empty;
            setPromptWidth();
            txtScan.Clear(); // does this cause the TextChanged event?
            lblScan.Text = "ITEM >>";
            lblProduct.Visible = false;
            lblItemQty.Visible = false;
            _entryMode = EntryMode.Product;
        }

        void showBulkItemPrompt()
        {
            setPromptWidth();
            txtScan.Text = "N";
            txtScan.SelectAll();
            lblScan.Text = "BULK? (y/n) >>";
            _entryMode = EntryMode.BulkItem;
        }

        void showItemTypePrompt()
        {
            setPromptWidth();
            txtScan.Clear();
            txtScan.Focus();
            lblScan.Text = "RF/NB? >>";
            _entryMode = EntryMode.ItemType;
        }

        void showProductDetails()
        {
            try
            {
                _qty = _engine.GetCalculatedInventory(
                _customer.CompCode,
                _customer.CustCode,
                _location.LocCode,
                _item.ItemCode);

                tick();
            }
            catch (Exception ex)
            {
                showError(ex.Message);
                showProductPrompt();
                _entryMode = EntryMode.ProductError;
            }
            
        }

        void showSerialNoPrompt()
        {
            _serialNo = string.Empty;
            _itemType = string.Empty;
            refreshProductHeader();
            setPromptWidth(.7M);
            txtScan.Clear();
            txtScan.Focus();
            lblScan.Text = "SERIAL >>";
            _entryMode = EntryMode.SerialNo;
            hideError();
        }

        void showRescanPrompt()
        {
            txtScan.Text = "N";
            txtScan.SelectAll();
            lblScan.Text = "Re-scan? >>";
            showWarning("All data for the location/item will be deleted.");
            _entryMode = EntryMode.AdminRescan;
        }

        #endregion

        #region private

        void refreshProductHeader()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(_item.ItemCode);
            if (this.IsBulkItem)
                sb.Append("   (B)");
            if (!string.IsNullOrEmpty(this.ItemType))
                sb.Append(string.Format(" -{0}", this.ItemType));
            lblProduct.Text = sb.ToString();
            lblProduct.Refresh();
        }

        void addedSerialNo()
        {
            _serialNo = string.Empty;
            tick();

            if (_entryMode != EntryMode.Finished &&
                _entryMode != EntryMode.AdminRescan)
                 showSerialNoPrompt();    
        }

        void tick()
        {
            int cnt = _engine.GetScannedCount(_item.CustCode, _location.LocCode, _item.ItemCode);

            if (cnt == _qty) // all items scanned check
            {
                // allow admin to rescan a location
                if (CiscoSnCycEngine.IsAdminUser(_opcode))
                {
                    showRescanPrompt();
                }
                else
                {
                    finish();
                    return;
                }
            }

            lblItemQty.Visible = true;
            lblItemQty.Text = string.Format("{0} of {1}", cnt, _qty);
        }

        void finish()
        {
            showError(string.Format("Item '{0}' scan completed", _item.ItemCode));

            switch (_entryMode)
            {
                case EntryMode.Product:
                case EntryMode.SerialNo:
                case EntryMode.ItemType:
                case EntryMode.Finished:
                    
                    _item = null;
                    
                    showProductPrompt();        
                    
                    _entryMode = EntryMode.Finished;

                    break;
            }            
        }

        void setPromptWidth()
        {
            setPromptWidth(.5M);
        }

        void setPromptWidth(decimal size)
        {
            txtScan.Width = pnlScan.Width - lblScan.Width;

            //txtScan.Width = (int)(pnlScan.Width * size);
        }

        void showError(string msg)
        {
            lblError.Visible = true;
            lblError.Text = msg;
            lblError.ForeColor = Color.Red;
        }

        void showWarning(string msg)
        {
            lblError.Visible = true;
            lblError.Text = msg;
            lblError.ForeColor = Color.Yellow;
        }
        
        void hideError()
        {
            lblError.Visible = false;
        }
        #endregion

        #region Back
        /// <summary>
        /// Controls the backwards navigation, user can go backwards from any step
        /// </summary>
        /// <returns></returns>
        bool Back()
        {
            if (ScanValue.Equals("X"))
            {
                hideError();

                switch (_entryMode)
                {
                    case EntryMode.SerialNo:
                    case EntryMode.BulkItem:
                    case EntryMode.ItemType:    
                        showProductPrompt();
                        break;

                    case EntryMode.Product:
                        showLocationPrompt();
                        break;
                    
                    case EntryMode.Location:
                        showCustomerPrompt();
                        break;
                    
                    case EntryMode.Customer:
                        _customer = null;
                        this.Close();
                        Application.Exit();
                        break;
                    
                    case EntryMode.Finished:
                    case EntryMode.AdminRescan:
                        showProductPrompt();
                        break;
                    
                    default:
                        return false;
                }
                return true;
            }

            return false;
        }
        #endregion

        #region Bulk
        bool Bulk()
        {
            if (ScanValue.Length == 1)
            {
                if (_entryMode == EntryMode.BulkItem)
                {
                    this.BulkItemFlag = ScanValue;
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Rescan
        bool Rescan()
        {
            if (_entryMode == EntryMode.AdminRescan)
            {
                if (this.ScanValue == "Y")
                {

                    try
                    {
                        _engine.RescanSerials(
                            _customer.CustCode,
                            _location.LocCode,
                            _item.ItemCode
                        );

                        System.Diagnostics.Debug.WriteLine(_itemType);

                        hideError();
                        if (string.IsNullOrEmpty(BulkItemFlag))
                            showBulkItemPrompt(); //this occurs if admin user just entered the Product
                        else
                            showSerialNoPrompt();
                        tick();
                    }
                    catch (Exception ex)
                    {
                        showError(ex.Message);
                        return false;
                    }

                    return true;
                }
                else
                {
                    // user chooses not to re-scan, so prompt for another item
                    hideError();
                    showProductPrompt();
                    return true;
                }
            }
            return false;
        }
        #endregion

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

    }
}
