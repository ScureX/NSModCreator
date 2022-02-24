using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace NSModCreator
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void OnCreateClicked(object sender, RoutedEventArgs e)
        {
            TextBox tb_folderPath = this.FindControl<TextBox>("tb_folderPath");
            TextBox tb_modName = this.FindControl<TextBox>("tb_modName");
            NumericUpDown tb_modLoadPriority = this.FindControl<NumericUpDown>("tb_modLoadPriority");
            CheckBox tb_modRequiredOnClient = this.FindControl<CheckBox>("tb_modRequiredOnClient");
            ComboBox tb_modRunOn = this.FindControl<ComboBox>("tb_modRunOn");
            TextBox tb_modVersion = this.FindControl<TextBox>("tb_modVersion");
            TextBox tb_modDescription = this.FindControl<TextBox>("tb_modDescription");
            Label lbl_status = this.FindControl<Label>("lbl_status");

            lbl_status.Content = "Getting Values...";

            // mod.json
            string path;
            string modName;
            string loadPriority;
            string requiredOnClient;
            string runOn;
            string version;
            string description;

            // file.nut
            string fileName;
            string initName;

            try
            {
                modName = tb_modName.Text.Trim();
                path = tb_folderPath.Text.Trim() + @"\" + modName;
                loadPriority = tb_modLoadPriority.Text.Trim();
                requiredOnClient = (bool)tb_modRequiredOnClient.IsChecked ? "true" : "false";
                version = tb_modVersion.Text.Trim();
                description = tb_modDescription.Text.Trim();
                TextBlock item = (TextBlock)tb_modRunOn.SelectedItem;
                runOn = item.Text;
            }
            catch (NullReferenceException ex)
            {
                lbl_status.Content = "Error: Some fields have incorrect values!";
                return;
            }

            

            fileName = modName.Replace(".", "_").ToLower();
            initName = fileName + "Init";
            if (modName.ToLower().Contains('.'))
            {
                initName = fileName.Split(".")[^1] + "Init";
            }

            /* format that shit */
            lbl_status.Content = "Creating Files...";
            // make folders
            if (Directory.Exists(path))
            {
                lbl_status.Content = "Error: Directory/Files already exists!";
                return;
            }

            Directory.CreateDirectory(path + @"\mod\scripts\vscripts");

            // mod.json
            using (FileStream fs = File.Create(path + @"\mod.json"))
            {
                byte[] info = { };
                // TODO check if this logic checks out, make only those 3 available
                if(runOn == "CLIENT")
                {
                    string res = GetEmbeddedResourceContent("NSModCreator.TEMPLATE_CLI_mod.json.txt", modName, loadPriority, requiredOnClient, version, description, fileName, initName);

                    info = new UTF8Encoding(true).GetBytes(res);
                }
                else if(runOn == "SERVER")
                {
                    string res = GetEmbeddedResourceContent("NSModCreator.TEMPLATE_SER_mod.json.txt", modName, loadPriority, requiredOnClient, version, description, fileName, initName);

                    info = new UTF8Encoding(true).GetBytes(res);
                }
                else if(runOn == "BOTH")
                {
                    string res = GetEmbeddedResourceContent("NSModCreator.TEMPLATE_CLISER_mod.json.txt", modName, loadPriority, requiredOnClient, version, description, fileName, initName);

                    info = new UTF8Encoding(true).GetBytes(res);
                }

                // Add some information to the file.
                fs.Write(info, 0, info.Length);
            }


            // file.nut
            using (FileStream fs = File.Create(path + @"\mod\scripts\vscripts\" + fileName + ".nut"))
            {
                string res = GetEmbeddedResourceContent("NSModCreator.TEMPLATE_file.nut.txt", modName, loadPriority, requiredOnClient, version, description, fileName, initName);
                byte[] info = new UTF8Encoding(true).GetBytes(res);
                
                // Add some information to the file.
                fs.Write(info, 0, info.Length);
            }

            /* cleanup*/
            lbl_status.Content = "Created Mod at " + path;
            ClearFields(tb_folderPath, tb_modName, tb_modLoadPriority, tb_modVersion, tb_modDescription);
        }

        public void OnClearClicked(object sender, RoutedEventArgs e)
        {
            TextBox tb_folderPath = this.FindControl<TextBox>("tb_folderPath");
            TextBox tb_modName = this.FindControl<TextBox>("tb_modName");
            NumericUpDown tb_modLoadPriority = this.FindControl<NumericUpDown>("tb_modLoadPriority");
            TextBox tb_modVersion = this.FindControl<TextBox>("tb_modVersion");
            TextBox tb_modDescription = this.FindControl<TextBox>("tb_modDescription");
            ClearFields(tb_folderPath, tb_modName, tb_modLoadPriority, tb_modVersion, tb_modDescription);
        }

        private void ClearFields(TextBox tb_folderPath, TextBox tb_modName, NumericUpDown tb_modLoadPriority, TextBox tb_modVersion,  TextBox tb_modDescription)
        {
            tb_folderPath.Text = "";
            tb_modName.Text = "";
            tb_modLoadPriority.Text = "1";
            tb_modVersion.Text = "1.0.0";
            tb_modDescription.Text = "";
        }

        public static string GetEmbeddedResourceContent(string resourceName, string modName, string loadPriority, string requiredOnClient, string version, string description, string fileName, string initName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            Stream stream = asm.GetManifestResourceStream(resourceName);
            StreamReader source = new StreamReader(stream);
            string fileContent = source.ReadToEnd()
                .Replace("PLACEHOLDER_NAME", modName)
                .Replace("PLACEHOLDER_DESC", description)
                .Replace("PLACEHOLDER_LOADPRIO", loadPriority)
                .Replace("PLACEHOLDER_REQONCLI", requiredOnClient)
                .Replace("PLACEHOLDER_VERSION", version)
                .Replace("PLACEHOLDER_FILENAME", fileName)
                .Replace("PLACEHOLDER_INTINAME", initName);
            source.Dispose();
            stream.Dispose();
            return fileContent;
        }
    }
}
