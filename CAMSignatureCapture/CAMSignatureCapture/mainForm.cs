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
    public partial class mainForm : Form
    {
        private Intermec.DataCollection2.ImagerCapture imgCapture;
        private Bitmap myImager;

        public mainForm()
        {
            InitializeComponent();
            loadDatacollection();
        }

        void loadDatacollection()
        {
            try
            {
                this.imgCapture = new Intermec.DataCollection2.ImagerCapture("Camera");

                if (!System.IO.Directory.Exists(@"\My Documents"))
                    System.IO.Directory.CreateDirectory(@"\My Documents");
                if (!System.IO.Directory.Exists(@"\My Documents\MDI"))
                    System.IO.Directory.CreateDirectory(@"\My Documents\MDI");
                imgCapture.CapturedImageLocation = @"\My Documents\MDI";

                //assign the camera to the scan button
                this.imgCapture.SetButtonAction(Intermec.DataCollection2.ImagerCapture.ButtonID.Center, Intermec.DataCollection2.ImagerCapture.ButtonActionType.Camera);

                this.imgCapture.Error += new ErrorEventHandler(imgCapture_Error);

                //see also form.resize
                Rectangle workRect = Screen.PrimaryScreen.WorkingArea;
                workRect = panel1.RectangleToScreen(panel1.ClientRectangle);
                //this.imgCapture.SetViewFinderCoordinates(8, 40, 480, 630);
                this.imgCapture.SetViewFinderCoordinates(workRect.Left, workRect.Top, workRect.Right, workRect.Bottom);
                
                this.imgCapture.ViewFinderEnable = true;
                this.imgCapture.TriggerEnable = Intermec.DataCollection2.ImagerCapture.TriggerEnableValue.Enable;

                this.imgCapture.FolderMemoryLimit = 30;
                if (imgCapture.SupportDocumentCapture)
                    imgCapture.DocumentCaptureEnable = ImagerCapture.DocumentCaptureEnableValue.Disable;

                if (this.imgCapture.SupportSigatureCapture)
                {
                    //enable signature capture
                    this.imgCapture.SignatureCapture += new SignatureCaptureEvenHandler(this.imgCapture_SignatureCapture);

                    //do not touch scenarios, use Intermec Settings or SigCapSettings
                    //this.imgCapture.SignatureScenario3.ScenarioEnable = SignatureScenario.ScenarioEnableType.Regular;
                    //this.imgCapture.SignatureScenario4.ScenarioEnable = SignatureScenario.ScenarioEnableType.Regular;
                    
                    this.imgCapture.SignatureFileNameTemplate = "sign_$(num)";
                    this.imgCapture.SignatureNotification = Intermec.DataCollection2.ImagerCapture.SignatureNotificationType.StoredImageInFiles;// StoredImageAndBarcodeInFiles;
                }
                else 
                {
                    MessageBox.Show("Sorry, no Camera Signature Support found");
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

        void imgCapture_Error(object sender, ErrorEventArgs CaptureErrorArgs)
        {

            setStatus(CaptureErrorArgs.ErrorMessage);
        }



        private void imgCapture_SignatureCapture(object sender, SignatureCaptureEventArgs SigCaptureEventArgs)
        {
            try
            {
                if (SigCaptureEventArgs.SignatureCaptureFile.Length > 0)
                {
                    //bitmap
                    if (this.myImager != null)
                    {
                        this.myImager.Dispose();
                    }
                    this.myImager = new Bitmap(SigCaptureEventArgs.SignatureCaptureFile);

                    //scaling
                    if (myImager.Size.Height > pictureSignature.Size.Height || myImager.Size.Width > pictureSignature.Size.Width)
                    {
                        myImager = resizeBitmap(myImager, pictureSignature.Size.Width, pictureSignature.Size.Height);
                    }

                    setStatus(SigCaptureEventArgs.SignatureCaptureFile);
                    //this.statusBar1.Text = Path.GetFileName(SigCaptureEventArgs.SignatureCaptureFile);
                    if (this.myImager != null)
                    {
                        this.pictureSignature.Show();
                        this.pictureSignature.Image = this.myImager;
                    }
                }
                else
                {
                    this.pictureSignature.Image = null;
                    setStatus("no file");
                }

                if(SigCaptureEventArgs.BarcodeData.Length>0)
                    setBarcodeTxt(SigCaptureEventArgs.BarcodeData);

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void mnuSettings_Click(object sender, EventArgs e)
        {
            SigCapSettings dlg = new SigCapSettings(ref imgCapture);
            dlg.ShowDialog();
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                Application.DoEvents();
                if (this.imgCapture != null)
                {
                    this.imgCapture.ViewFinderEnable = false;
                    //restore scan button
                    this.imgCapture.SetButtonAction(Intermec.DataCollection2.ImagerCapture.ButtonID.Center, Intermec.DataCollection2.ImagerCapture.ButtonActionType.Scan);

                    //???
                    if (this.imgCapture.SupportSigatureCapture)
                    {
                        //this.imgCapture.SignatureScenario1.ScenarioEnable = SignatureScenario.ScenarioEnableType.Disabled;
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
            Cursor.Current = Cursors.Default;
            Application.DoEvents();
            Application.Exit();
        }

        private void mainForm_Resize(object sender, EventArgs e)
        {
            if(this.imgCapture!=null)
                this.imgCapture.ViewFinderEnable = false;
            Rectangle workRect = Screen.PrimaryScreen.WorkingArea;
            workRect = panel1.RectangleToScreen(panel1.ClientRectangle);
            //this.imgCapture.SetViewFinderCoordinates(8, 40, 480, 630);
            if (this.imgCapture != null)
            {
                this.imgCapture.SetViewFinderCoordinates(workRect.Left, workRect.Top, workRect.Right, workRect.Bottom);
                this.imgCapture.ViewFinderEnable = true;
            }
        }

        void setStatus(string message)
        {
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

        void setBarcodeTxt(string sBarcode)
        {
            Action method = delegate
            {
                this.lblBarcode.Text = sBarcode;
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

        private void menuOptions_Click(object sender, EventArgs e)
        {

        }
        
        public static Bitmap resizeBitmap(Bitmap original, int newX, int newY)
        {
            Rectangle recSrc = new Rectangle(0, 0, original.Width, original.Height);
            Rectangle recDest = new Rectangle(0, 0, newX, newY);
            Bitmap target = new Bitmap(newX, newY);
            Graphics g = Graphics.FromImage(target);
            g.DrawImage(original, recDest, recSrc, GraphicsUnit.Pixel);
            return target;
        }
    }
}