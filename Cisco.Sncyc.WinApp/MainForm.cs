using Accellos.Business.Contracts;
using Accellos.Business.Entities;
using Accellos.Data.Contracts;
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

                if (_engine.IsProductValid(_item))
                {
                    if (_item.Serialized)
                    {
                        hideError();
                        this.lblProduct.Text = _item.ItemCode;
                        lblProduct.Visible = true;
                        showProductDetails();

                        if (_entryMode != EntryMode.Finished && 
                            _entryMode != EntryMode.AdminRescan)     
                            showBulkItemPrompt();

                        //if (_entryMode == EntryMode.AdminRescan)


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

        private void txtScan_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                if (Back()) return;
                if (Bulk()) return;
                if (Rescan()) return;
            }
        }

        #region txtScan_TextChanged
        private void txtScan_TextChanged(object sender, EventArgs e)
        {
            
            if (ScanValue.Length == 0) {
                return;
            }

            if (ScanValue.Length > 1)
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
                            hideError();
                            showItemTypePrompt();
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
            }
            
        }
        #endregion

        #region show

        void showCustomerPrompt()
        {
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
            _entryMode = EntryMode.Location;
        }

        void showProductPrompt()
        {
            _item = null; //reset the variable so next scan will take
            setPromptWidth();
            txtScan.Clear(); // does this cause the TextChanged event?
            lblScan.Text = "Item >>";
            lblProduct.Visible = false;
            lblItemQty.Visible = false;
            _entryMode = EntryMode.Product;
        }

        void showBulkItemPrompt()
        {
            setPromptWidth();
            txtScan.Text = "N";
            txtScan.SelectAll();
            lblScan.Text = "Bulk? (y/n) >>";
            _entryMode = EntryMode.BulkItem;
        }

        void showItemTypePrompt()
        {
            setPromptWidth();
            txtScan.Clear();
            txtScan.Focus();
            lblScan.Text = "Item Type >>";
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
                _item = null;
            }
            
        }

        void refreshProductHeader()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(_item.ItemCode);
            if (this.IsBulkItem)
                sb.Append(" -B"); 
            if (!string.IsNullOrEmpty(this.ItemType))
                sb.Append(string.Format(" -{0}", this.ItemType));
            lblProduct.Text = sb.ToString();
        }

        void showSerialNoPrompt()
        {
            _serialNo = string.Empty;
            _itemType = string.Empty;
            refreshProductHeader();
            setPromptWidth(.6M);
            txtScan.Clear();
            txtScan.Focus();
            lblScan.Text = "SerialNo >>";
            _entryMode = EntryMode.SerialNo;
            hideError();
        }

        void showRescanPrompt()
        {
            txtScan.Text = "N";
            txtScan.SelectAll();
            lblScan.Text = "Re-scan? >>";
            showWarning("All data for the item and location will be deleted.");
            _entryMode = EntryMode.AdminRescan;
        }

        #endregion

        #region private

        void addedSerialNo()
        {
            _serialNo = string.Empty;
            tick();
            if (_entryMode != EntryMode.Finished) 
                showSerialNoPrompt();
        }

        void tick()
        {
            int cnt = _engine.GetScannedCount(_item.CustCode, _location.LocCode, _item.ItemCode);

            if (cnt == _qty) // check if all items scanned
            {
                // allow admin to rescan a location
                if (_engine.IsAdminUser(_opcode))
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
            showError(string.Format("Item '{0}' already scanned", _item.ItemCode));

            switch (_entryMode)
            {
                case EntryMode.Product:
                case EntryMode.SerialNo:
                case EntryMode.Finished:
                    
                    _item = null;
                    
                    showProductPrompt();        
                    
                    _entryMode = EntryMode.Finished;

                    break;
            }            
        }

        void setPromptWidth()
        {
            setPromptWidth(.4M);
        }

        void setPromptWidth(decimal size)
        {
            txtScan.Width = (int)(pnlScan.Width * size);
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
        /// check for user initiated Back event
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
                        _item = null;
                        _itemType = string.Empty;
                        _bulkItemFlag = string.Empty;
                        showProductPrompt();
                        break;
                    case EntryMode.Product:
                    case EntryMode.ItemType:
                        _item = null;
                        _location = null;
                        _itemType = string.Empty;
                        _bulkItemFlag = string.Empty;
                        showLocationPrompt();
                        break;
                    case EntryMode.Location:
                        _location = null;
                        _customer = null;
                        showCustomerPrompt();
                        break;
                    case EntryMode.Customer:
                        _customer = null;
                        this.Close();
                        Application.Exit();
                        break;
                    case EntryMode.Finished:
                    case EntryMode.AdminRescan:
                        _item = null;
                        _qty = 0;
                        //go back to the Product prompt but in a finished state
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

        bool Rescan()
        {
            if (this.ScanValue == "Y")
            {
                if (_entryMode == EntryMode.AdminRescan)
                {
                    try
                    {
                        _engine.RescanSerials(
                            _customer.CustCode,
                            _location.LocCode,
                            _item.ItemCode
                        );

                        hideError();
                        showBulkItemPrompt();
                        tick();
                    }
                    catch (Exception ex)
                    {
                        showError(ex.Message);
                        return false;
                    }
                    
                    return true;
                }
            }
            return false;
        }
    }
}
