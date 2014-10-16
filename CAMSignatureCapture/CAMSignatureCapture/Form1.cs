using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Intermec.DataCollection2;

namespace CAMSignatureCapture
{
    public class Form1 : Form
    {
        // Fields
        private IContainer components = null;
        private Intermec.DataCollection2.ImagerCapture imgCapture;
        private MainMenu mainMenu1;
        private MenuItem menuOptions;
        private MenuItem mnuExit;
        private Bitmap myImager;
        private PictureBox pictureDocument;
        private PictureBox pictureSignature;
        private MenuItem mnuSettings;
        private StatusBar statusBar1;

        // Methods
        public Form1()
        {
            this.InitializeComponent();
            try
            {
                this.imgCapture = new Intermec.DataCollection2.ImagerCapture("Camera");
                this.imgCapture.SetButtonAction(Intermec.DataCollection2.ImagerCapture.ButtonID.Center, Intermec.DataCollection2.ImagerCapture.ButtonActionType.Camera);
                this.imgCapture.SetViewFinderCoordinates(8, 40, 480, 630);
                this.imgCapture.ViewFinderEnable = true;
                this.imgCapture.TriggerEnable = Intermec.DataCollection2.ImagerCapture.TriggerEnableValue.Enable;
                this.imgCapture.FolderMemoryLimit = 30;
                //this.mnuExit.Enabled = false;
                if (this.imgCapture.SupportSigatureCapture)
                {
                    this.imgCapture.SignatureCapture += new SignatureCaptureEvenHandler(this.imgCapture_SignatureCapture);
                    this.imgCapture.SignatureScenario3.ScenarioEnable = SignatureScenario.ScenarioEnableType.Regular;
                    this.imgCapture.SignatureScenario4.ScenarioEnable = SignatureScenario.ScenarioEnableType.Regular;
                    this.imgCapture.SignatureFileNameTemplate = "sign_$(num)";
                    this.imgCapture.SignatureNotification = Intermec.DataCollection2.ImagerCapture.SignatureNotificationType.StoredImageInFiles;
                }
            }
            catch (ImagerCaptureException exception)
            {
                MessageBox.Show("ImagerCaptureException: " + exception.ToString());
            }
            catch (Exception exception2)
            {
                MessageBox.Show(exception2.ToString());
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void imgCapture_DocumentCapture(object sender, DocumentCaptureEventArgs DocCaptureEventArgs)
        {
            try
            {
                if (DocCaptureEventArgs.DocCapFile.Length > 0)
                {
                    if (this.myImager != null)
                    {
                        this.myImager.Dispose();
                    }
                    this.myImager = new Bitmap(DocCaptureEventArgs.DocCapFile);
                    if (this.myImager != null)
                    {
                        this.pictureDocument.Show();
                        this.pictureSignature.Hide();
                        this.pictureDocument.Image = this.myImager;
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void imgCapture_Guidance(object sender, GuidanceEventArgs args)
        {
            string message = "";
            if (args.FocusCheckFailure)
            {
                message = "Not in focus";
            }
            else if (args.ImagerFar)
            {
                message = "Move closer";
            }
            else if (args.ImagerSharp)
            {
                message = "Angle too sharp";
            }
            else if (args.InternalError)
            {
                message = "Internal error";
            }
            Action method = delegate
            {
                this.statusBar1.Text = message;
            };
            if (this.InvokeRequired)
            {
                base.Invoke(method);
            }
            else
            {
                method();
            }
        }

        private void imgCapture_SignatureCapture(object sender, SignatureCaptureEventArgs SigCaptureEventArgs)
        {
            try
            {
                if (SigCaptureEventArgs.SignatureCaptureFile.Length > 0)
                {
                    if (this.myImager != null)
                    {
                        this.myImager.Dispose();
                    }
                    this.myImager = new Bitmap(SigCaptureEventArgs.SignatureCaptureFile);
                    this.statusBar1.Text = Path.GetFileName(SigCaptureEventArgs.SignatureCaptureFile);
                    if (this.myImager != null)
                    {
                        this.pictureSignature.Show();
                        this.pictureDocument.Hide();
                        this.pictureSignature.Image = this.myImager;
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void imgCapture_SnapShotCapture(object sender, SnapShotCaptureEventArgs SnapCaptureEventArgs)
        {
            try
            {
                if (this.myImager != null)
                {
                    this.myImager.Dispose();
                }
                this.myImager = new Bitmap(SnapCaptureEventArgs.SnapshotFile);
                if (this.myImager != null)
                {
                    this.pictureDocument.Show();
                    this.pictureSignature.Hide();
                    this.pictureDocument.Image = this.myImager;
                }
            }
            catch (OutOfMemoryException exception)
            {
                string message = exception.Message;
                if (SnapCaptureEventArgs.SnapshotFile.Length > 0)
                {
                    this.statusBar1.Text = "snapImage: " + SnapCaptureEventArgs.SnapshotFile;
                }
            }
            catch (Exception exception2)
            {
                MessageBox.Show(exception2.Message);
            }
        }

        private void InitializeComponent()
        {
            this.pictureSignature = new System.Windows.Forms.PictureBox();
            this.pictureDocument = new System.Windows.Forms.PictureBox();
            this.statusBar1 = new System.Windows.Forms.StatusBar();
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.mnuExit = new System.Windows.Forms.MenuItem();
            this.menuOptions = new System.Windows.Forms.MenuItem();
            this.mnuSettings = new System.Windows.Forms.MenuItem();
            this.SuspendLayout();
            // 
            // pictureSignature
            // 
            this.pictureSignature.Location = new System.Drawing.Point(17, 10);
            this.pictureSignature.Name = "pictureSignature";
            this.pictureSignature.Size = new System.Drawing.Size(206, 227);
            this.pictureSignature.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            // 
            // pictureDocument
            // 
            this.pictureDocument.BackColor = System.Drawing.Color.Transparent;
            this.pictureDocument.Location = new System.Drawing.Point(26, 5);
            this.pictureDocument.Name = "pictureDocument";
            this.pictureDocument.Size = new System.Drawing.Size(188, 237);
            this.pictureDocument.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            // 
            // statusBar1
            // 
            this.statusBar1.Location = new System.Drawing.Point(0, 246);
            this.statusBar1.Name = "statusBar1";
            this.statusBar1.Size = new System.Drawing.Size(240, 22);
            this.statusBar1.Text = "Status:";
            // 
            // mnuExit
            // 
            this.mnuExit.Text = "Exit";
            this.mnuExit.Click += new System.EventHandler(this.menuExit_Click_1);
            // 
            // menuOptions
            // 
            this.menuOptions.MenuItems.Add(this.mnuSettings);
            this.menuOptions.Text = "Options";
            this.menuOptions.Click += new System.EventHandler(this.menuExit_Click_1);
            // 
            // mnuSettings
            // 
            this.mnuSettings.Text = "Settings";
            this.mnuSettings.Click += new System.EventHandler(this.mnuSettings_Click);
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.mnuExit);
            this.mainMenu1.MenuItems.Add(this.menuOptions);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.ControlBox = false;
            this.Controls.Add(this.statusBar1);
            this.Controls.Add(this.pictureSignature);
            this.Controls.Add(this.pictureDocument);
            this.Menu = this.mainMenu1;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "Press Trigger";
            this.ResumeLayout(false);

        }

        private void menuExit_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (this.imgCapture != null)
                {
                    this.imgCapture.ViewFinderEnable = false;
                    this.imgCapture.SetButtonAction(Intermec.DataCollection2.ImagerCapture.ButtonID.Center, Intermec.DataCollection2.ImagerCapture.ButtonActionType.Scan);
                    if (this.imgCapture.SupportSigatureCapture)
                    {
                        this.imgCapture.SignatureScenario1.ScenarioEnable = SignatureScenario.ScenarioEnableType.Disabled;
                    }
                    if (this.imgCapture.SupportDocumentCapture)
                    {
                        this.imgCapture.DocumentCaptureEnable = Intermec.DataCollection2.ImagerCapture.DocumentCaptureEnableValue.Disable;
                    }
                    this.imgCapture.Dispose();
                    this.imgCapture = null;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            Application.Exit();
        }

        private void menuSnapShot_Click(object sender, EventArgs e)
        {

        }

        private void mnuSettings_Click(object sender, EventArgs e)
        {
            SigCapSettings dlg = new SigCapSettings(ref imgCapture);
            if (dlg.ShowDialog() == DialogResult.OK)
                ;// imgCapture.SignatureScenario1 = dlg.scenario; //change settings

        }
    }
}
 
