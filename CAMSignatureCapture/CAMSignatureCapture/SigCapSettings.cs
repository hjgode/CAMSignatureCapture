using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

using Intermec.DataCollection2;

namespace CAMSignatureCapture
{
    public partial class SigCapSettings : Form
    {
        Intermec.DataCollection2.ImagerCapture imgCap;
        Intermec.DataCollection2.SignatureScenario scenario;
        public SigCapSettings(ref Intermec.DataCollection2.ImagerCapture imgCapIn)
        {
            InitializeComponent();
            listScenarios.Items.Clear();
            for (int i = 1; i < 5; i++)
                listScenarios.Items.Add(i.ToString());
            listScenarios.SelectedIndex = 0;

            imgCap = imgCapIn;
            this.scenario = imgCap.SignatureScenario1;
            updateUI();
            listScenarios.SelectedIndexChanged += new EventHandler(listScenarios_SelectedIndexChanged);
            
        }

        void listScenarios_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!bLoading)
            {
                if (bValueChanged)
                {
                    if (MessageBox.Show("Discard changes?", "change scenario", MessageBoxButtons.YesNo, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        saveChanges();
                        bValueChanged = false;
                    }
                }
            }
                
            switch (listScenarios.SelectedIndex)
            {
                case 0:
                    this.scenario = imgCap.SignatureScenario1;
                    break;
                case 1:
                    this.scenario = imgCap.SignatureScenario2;
                    break;
                case 2:
                    this.scenario = imgCap.SignatureScenario3;
                    break;
                case 3:
                    this.scenario = imgCap.SignatureScenario4;
                    break;
            }
            updateUI();
        }

        void updateUI()
        {
            bLoading = true;
            Enum[] myvalues = EnumToArray(new SignatureScenario.BarcodeIdentifierType());
            int idx = -1, found=-1;
            listBarcodeIDs.Items.Clear();
            foreach (Enum e in myvalues)
            {
                idx=listBarcodeIDs.Items.Add(e.ToString());
                if (e.ToString() == scenario.ScenarioBarcodeIdentifier.ToString())
                    found = idx;
            }
            if (found != -1)
                listBarcodeIDs.SelectedIndex = found;

            numBCLength.Value = scenario.ScenarioBarcodeLength;

            txtBCMask.Text = scenario.ScenarioBarcodeMask;

            numBCHeight.Value = scenario.ScenarioBarcodeHeight;
            numBCWidth.Value = scenario.ScenarioBarcodeWidth;

            numAreaHeight.Value = scenario.ScenarioAreaHeight;
            numAreaWidth.Value = scenario.ScenarioAreaWidth;

            numAreaHorizontalOffset.Value = scenario.ScenarioHorizontalOffset;
            numAreaVerticalOffset.Value = scenario.ScenarioVerticalOffset;

            //Scenario Enabled settings
            listScenarioEnabled.Items.Clear();
            Enum[] enabledList = EnumToArray(new Intermec.DataCollection2.SignatureScenario.ScenarioEnableType());
            idx = -1; found = -1;
            foreach (Enum e in enabledList)
            {
                idx = listScenarioEnabled.Items.Add(e.ToString());
                if (e.ToString() == scenario.ScenarioEnable.ToString())
                    found = idx;
            }
            if(found!=-1)
                listScenarioEnabled.SelectedIndex = found;
            else
                listScenarioEnabled.SelectedIndex = 0;

            //automatic Image Correction
            listCorrections.Items.Clear();
            Enum[] imageCorrections = EnumToArray(new Intermec.DataCollection2.SignatureScenario.AutomaticCorrectionType());
            idx = -1; found = -1;
            foreach (Enum e in imageCorrections)
            {
                idx = listCorrections.Items.Add(e.ToString());
                if (e.ToString() == scenario.ScenarioAutomaticCorrection.ToString())
                    found = idx;
            }
            if(found!=-1)
                listCorrections.SelectedIndex = found;
            else
                listCorrections.SelectedIndex = 0;

            //focus check
            listFocusCheck.Items.Clear();
            Enum[] focusCheck = EnumToArray(new Intermec.DataCollection2.SignatureScenario.FocusType());
            idx = -1; found = -1;
            foreach (Enum e in focusCheck)
            {
                idx = listFocusCheck.Items.Add(e.ToString());
                if (e.ToString() == scenario.ScenarioFocusCheck.ToString())
                    found = idx;
            }
            if(found!=-1)
                listFocusCheck.SelectedIndex = found;
            else
                listFocusCheck.SelectedIndex = 0;

            //brightness
            numBrightness.Value = scenario.ScenarioBrightness;

            //color conversions
            listColorConversions.Items.Clear();
            Enum[] colorConversions = EnumToArray(new Intermec.DataCollection2.ImageConditioning.ColorConversionValue());
            idx = -1; found = -1;
            foreach (Enum e in colorConversions)
            {
                idx = listColorConversions.Items.Add(e.ToString());
                if (e.ToString() == scenario.ScenarioColorConversion.ToString())
                    found = idx;
            }
            if(found!=-1)
                listColorConversions.SelectedIndex = found;
            else
                listColorConversions.SelectedIndex = 0;

            //color conversion treshold
            listColorConversionTresholds.Items.Clear();
            Enum[] colorConversionTresholds = EnumToArray(new Intermec.DataCollection2.ImageConditioning.ColorConversionBrightnessThresholdValue());
            idx = -1; found = -1;
            foreach (Enum e in colorConversionTresholds)
            {
                idx = listColorConversionTresholds.Items.Add(e.ToString());
                if (e.ToString() == scenario.ScenarioColorConversionBrightnessThreshold.ToString())
                    found = idx;
            }
            if(found!=-1)
                listColorConversionTresholds.SelectedIndex = found;
            else
                listColorConversionTresholds.SelectedIndex = 0;

            //contrast enhancements
            listContrastEnhancements.Items.Clear();
            Enum[] contrastEnhancements = EnumToArray(new Intermec.DataCollection2.ImageConditioning.ContrastEnhancementValue());
            idx = -1; found = -1;
            foreach (Enum e in contrastEnhancements)
            {
                idx = listContrastEnhancements.Items.Add(e.ToString());
                if (e.ToString() == scenario.ScenarioContrastEnhancement.ToString())
                    found = idx;
            }
            if(found!=-1)
                listContrastEnhancements.SelectedIndex = found;
            else
                listContrastEnhancements.SelectedIndex = 0;

            //image lightning correction
            listLightningCorrection.Items.Clear();
            Enum[] lightningCorrections = EnumToArray(new Intermec.DataCollection2.ImageConditioning.ImageLightingCorrectionValue());
            idx = -1; found = -1;
            foreach (Enum e in lightningCorrections)
            {
                idx = listLightningCorrection.Items.Add(e.ToString());
                if (e.ToString() == scenario.ScenarioImageLightingCorrection.ToString())
                    found = idx;
            }
            if(found!=-1)
                listLightningCorrection.SelectedIndex = found;
            else
                listLightningCorrection.SelectedIndex = 0;

            //image ratio check 0-0x64
            numImageRatioCheck.Value = scenario.ScenarioImageRatioCheck;

            //image rotation
            listImageRotations.Items.Clear();
            Enum[] imageRotations = EnumToArray(new Intermec.DataCollection2.ImageConditioning.ImageRotationValue());
            idx = -1; found = -1;
            foreach (Enum e in imageRotations)
            {
                idx = listImageRotations.Items.Add(e.ToString());
                if (e.ToString() == scenario.ScenarioImageRotation.ToString())
                    found = idx;
            }
            if(found!=-1)
                listImageRotations.SelectedIndex = found;
            else
                listImageRotations.SelectedIndex = 0;

            //noise reduction
            numNoiseReduction.Value = scenario.ScenarioNoiseReduction;

            //output compression
            listOutputCompressions.Items.Clear();
            Enum[] outputCompressions = EnumToArray(new Intermec.DataCollection2.ImageConditioning.OutputCompressionValue());
            idx = -1; found = -1;
            foreach (Enum e in outputCompressions)
            {
                idx = listOutputCompressions.Items.Add(e.ToString());
                if (e.ToString() == scenario.ScenarioOutputCompression.ToString())
                    found = idx;
            }
            if(found!=-1)
                listOutputCompressions.SelectedIndex = found;
            else
                listOutputCompressions.SelectedIndex = 0;
            
            //output compression quality
            numOutputCompressionQuality.Value = scenario.ScenarioOutputCompressionQuality;

            //ProjectiveMappingResolution
            listProjectiveMappingResolutions.Items.Clear();
            Enum[] projectiveMappingResolutions = EnumToArray(new Intermec.DataCollection2.SignatureScenario.ProjectiveMappingResolutionType());
            idx = -1; found = -1;
            foreach (Enum e in projectiveMappingResolutions)
            {
                idx = listProjectiveMappingResolutions.Items.Add(e.ToString());
                if (e.ToString() == scenario.ScenarioProjectiveMappingResolution.ToString())
                    found = idx;
            }
            if(found!=-1)
                listProjectiveMappingResolutions.SelectedIndex = found;
            else
                listProjectiveMappingResolutions.SelectedIndex = 0;

            //reverse image
            listReverseImage.Items.Clear();
            Enum[] reverseImage = EnumToArray(new Intermec.DataCollection2.ImageConditioning.ReverseVideoValue());
            idx = -1; found = -1;
            foreach (Enum e in reverseImage)
            {
                idx = listReverseImage.Items.Add(e.ToString());
                if (e.ToString() == scenario.ScenarioReverseImage.ToString())
                    found = idx;
            }
            if(found!=-1)
                listReverseImage.SelectedIndex = found;
            else
                listReverseImage.SelectedIndex = 0;

            //TextEnhancment
            listTextEnhancments.Items.Clear();
            Enum[] textEnhancments = EnumToArray(new Intermec.DataCollection2.ImageConditioning.TextEnhancementValue());
            idx = -1; found = -1;
            foreach (Enum e in textEnhancments)
            {
                idx = listTextEnhancments.Items.Add(e.ToString());
                if (e.ToString() == scenario.ScenarioTextEnhancement.ToString())
                    found = idx;
            }
            if(found!=-1)
                listTextEnhancments.SelectedIndex = found;
            else
                listTextEnhancments.SelectedIndex = 0;

            bLoading = false;
            bValueChanged=false;
        }

        void saveChanges()
        {
            scenario.ScenarioBarcodeIdentifier = 
                (SignatureScenario.BarcodeIdentifierType)Enum.Parse(typeof(SignatureScenario.BarcodeIdentifierType), listBarcodeIDs.SelectedItem.ToString(),true);
            scenario.ScenarioBarcodeLength = (int)numBCLength.Value;
            scenario.ScenarioBarcodeMask = txtBCMask.Text;
            scenario.ScenarioBarcodeHeight = (int)numBCHeight.Value;
            scenario.ScenarioBarcodeWidth = (int)numBCWidth.Value;

            scenario.ScenarioAreaHeight = (int)numAreaHeight.Value;
            scenario.ScenarioAreaWidth = (int)numAreaWidth.Value;
            scenario.ScenarioHorizontalOffset = (int)numAreaHorizontalOffset.Value;
            scenario.ScenarioVerticalOffset = (int)numAreaVerticalOffset.Value;

            //Scenario Enabled settings
            scenario.ScenarioEnable = (SignatureScenario.ScenarioEnableType)Enum.Parse(typeof(SignatureScenario.ScenarioEnableType), listScenarioEnabled.SelectedItem.ToString(), true);
            //automatic Image Correction
            scenario.ScenarioAutomaticCorrection = (SignatureScenario.AutomaticCorrectionType)Enum.Parse(typeof(SignatureScenario.AutomaticCorrectionType), listCorrections.SelectedItem.ToString(), true);
            //focus check
            scenario.ScenarioFocusCheck = (SignatureScenario.FocusType)Enum.Parse(typeof(SignatureScenario.FocusType), listFocusCheck.SelectedItem.ToString(), true);
            //brightness
            scenario.ScenarioBrightness = (int)numBrightness.Value;
            //color conversion
            scenario.ScenarioColorConversion = getEnumTypedValue<ImageConditioning.ColorConversionValue>(listColorConversions);
            //color conversion treshold
            scenario.ScenarioColorConversionBrightnessThreshold = getEnumTypedValue<ImageConditioning.ColorConversionBrightnessThresholdValue>(listColorConversionTresholds);
            //contrast enhancements
            scenario.ScenarioContrastEnhancement = getEnumTypedValue<ImageConditioning.ContrastEnhancementValue>(listContrastEnhancements);
            //image lightning correction
            scenario.ScenarioImageLightingCorrection = getEnumTypedValue<ImageConditioning.ImageLightingCorrectionValue>(listLightningCorrection);
            //image ratio check
            scenario.ScenarioImageRatioCheck = (int)numImageRatioCheck.Value;
            //image rotation
            scenario.ScenarioImageRotation = getEnumTypedValue<ImageConditioning.ImageRotationValue>(listImageRotations);
            //noise reduction
            scenario.ScenarioNoiseReduction = (int)numNoiseReduction.Value;
            //output compression
            scenario.ScenarioOutputCompression = getEnumTypedValue<ImageConditioning.OutputCompressionValue>(listOutputCompressions);
            //output compression quality
            scenario.ScenarioOutputCompressionQuality=(int)numOutputCompressionQuality.Value;
            //ProjectiveMappingResolution
            scenario.ScenarioProjectiveMappingResolution = getEnumTypedValue<SignatureScenario.ProjectiveMappingResolutionType>(listProjectiveMappingResolutions);
            //reverse image
            scenario.ScenarioReverseImage=getEnumTypedValue<ImageConditioning.ReverseVideoValue>(listReverseImage);
            //TextEnhancment
            scenario.ScenarioTextEnhancement = getEnumTypedValue<ImageConditioning.TextEnhancementValue>(listTextEnhancments);
            MessageBox.Show("values saved");
            bValueChanged = false;
        }

        static T getEnumTypedValue<T>(ComboBox cb)
        {
            return (T)Enum.Parse(typeof(T), cb.SelectedItem.ToString(), true);
        }
        
        private void mnuDone_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public Enum[] EnumToArray(Enum enumeration)
        {
            //get the enumeration type
            Type et = enumeration.GetType();

            //get the public static fields (members of the enum)
            System.Reflection.FieldInfo[] fi = et.GetFields(BindingFlags.Static | BindingFlags.Public);

            //create a new enum array
            Enum[] values = new Enum[fi.Length];

            //populate with the values
            for (int iEnum = 0; iEnum < fi.Length; iEnum++)
            {
                values[iEnum] = (Enum)fi[iEnum].GetValue(enumeration);
            }

            //return the array
            return values;
        }

        private void mnuSave_Click(object sender, EventArgs e)
        {
            saveChanges();
        }

        private void valueChanged(object sender, EventArgs e)
        {
            if (!bLoading)
                bValueChanged=true;

        }
        bool bLoading = true;
        bool bValueChanged = false;

        private void listScenarios_Validating(object sender, CancelEventArgs e)
        {

        }
    }
}