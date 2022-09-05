using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CryptDecrypt;


namespace WCrypt
{
    public partial class WCrypt : Form
    {
        public WCrypt()
        {
            InitializeComponent();
        }

        private void btnEncriptar_Click(object sender, EventArgs e)
        {
            if ( txtClave.Text.Trim() != "" && txtAEncriptar.Text.Trim() != "" )
            {
                txtEncriptado.Text = Crypt.Encrypt(txtAEncriptar.Text.Trim(), txtClave.Text.Trim());
            }
            
        }

        private void btnDesencriptar_Click(object sender, EventArgs e)
        {
            if ( txtEncriptado.Text.Trim() != "" )
            {
                txtTextoDesencriptado.Text = Crypt.Decrypt(txtEncriptado.Text.Trim(), txtClave.Text.Trim());
            }
        }
    }
}
