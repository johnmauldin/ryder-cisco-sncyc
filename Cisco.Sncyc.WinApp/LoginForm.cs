using Accellos.Business.Contracts;
using Core.Common.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cisco.Sncyc.WinApp
{
    public partial class LoginForm : Form
    {
        [Import]
        ILoginEngine _engine = null;

        public LoginForm()
        {
            InitializeComponent();
            ObjectBase.Container.SatisfyImportsOnce(this);
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            txtOpCode.Focus();
            this.AcceptButton = cmdOk;
        }

        private void cmdOk_Click(object sender, EventArgs e)
        {
            string opcode = txtOpCode.Text;

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                _engine.Authenticate(opcode, txtPassword.Text);

                this.Hide();

                MainForm main = new MainForm(opcode);
                main.Show();

            }
            catch (Exception ex)
            {
                lblError.Visible = true;
                lblError.Text = ex.Message;
                
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
            
        }

        private void LoginForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        
    }
}
