using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace NSModCreator
{
    public partial class MainWindow : Window
    {
        TextBox tb_folderPathOutput;
        TextBox tb_folderPathPublish;
        TextBox tb_modNamePublish;
        TextBox tb_modVersionPublish;
        TextBox tb_modWebsitePublish;
        TextBox tb_modDescriptionPublish;
        TextBox tb_modDependenciesPublish;
        Label lbl_statusPublish;
        TextBox tb_iconPathPublish;

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
            tb_folderPathOutput = this.FindControl<TextBox>("tb_folderPathOutput");
            tb_folderPathPublish = this.FindControl<TextBox>("tb_folderPathPublish");
            tb_modNamePublish = this.FindControl<TextBox>("tb_modNamePublish");
            tb_modVersionPublish = this.FindControl<TextBox>("tb_modVersionPublish");
            tb_modWebsitePublish = this.FindControl<TextBox>("tb_modWebsitePublish");
            tb_modDescriptionPublish = this.FindControl<TextBox>("tb_modDescriptionPublish");
            tb_modDependenciesPublish = this.FindControl<TextBox>("tb_modDependenciesPublish");
            lbl_statusPublish = this.FindControl<Label>("lbl_statusPublish");
            tb_iconPathPublish = this.FindControl<TextBox>("tb_iconPathPublish");
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
                path = tb_folderPath.Text.Trim() + @"/" + modName;
                loadPriority = tb_modLoadPriority.Text.Trim();
                requiredOnClient = (bool) tb_modRequiredOnClient.IsChecked ? "true" : "false";
                version = tb_modVersion.Text.Trim();
                description = tb_modDescription.Text.Trim();
                TextBlock item = (TextBlock) tb_modRunOn.SelectedItem;
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

            Directory.CreateDirectory(path + @"/mod/scripts/vscripts");

            // mod.json
            using (FileStream fs = File.Create(path + @"/mod.json"))
            {
                byte[] info = { };
                // TODO check if this logic checks out, make only those 3 available
                if (runOn == "CLIENT")
                {
                    string res = GetEmbeddedResourceContent("NSModCreator.TEMPLATE_CLI_mod.json.txt", modName,
                        loadPriority, requiredOnClient, version, description, fileName, initName);

                    info = new UTF8Encoding(true).GetBytes(res);
                }
                else if (runOn == "SERVER")
                {
                    string res = GetEmbeddedResourceContent("NSModCreator.TEMPLATE_SER_mod.json.txt", modName,
                        loadPriority, requiredOnClient, version, description, fileName, initName);

                    info = new UTF8Encoding(true).GetBytes(res);
                }
                else if (runOn == "BOTH")
                {
                    string res = GetEmbeddedResourceContent("NSModCreator.TEMPLATE_CLISER_mod.json.txt", modName,
                        loadPriority, requiredOnClient, version, description, fileName, initName);

                    info = new UTF8Encoding(true).GetBytes(res);
                }
                fs.Write(info, 0, info.Length);
            }


            // file.nut
            using (FileStream fs = File.Create(path + @"/mod/scripts/vscripts/" + fileName + ".nut"))
            {
                string res = GetEmbeddedResourceContent("NSModCreator.TEMPLATE_file.nut.txt", modName, loadPriority,
                    requiredOnClient, version, description, fileName, initName);
                byte[] info = new UTF8Encoding(true).GetBytes(res);
                fs.Write(info, 0, info.Length);
            }

            /* cleanup*/
            lbl_status.Content = "Created Mod at " + path.Replace("\\", "/");
            ClearFieldsCreate(tb_folderPath, tb_modName, tb_modLoadPriority, tb_modVersion, tb_modDescription);
        }

        public void OnClearClicked(object sender, RoutedEventArgs e)
        {
            TextBox tb_folderPath = this.FindControl<TextBox>("tb_folderPath");
            TextBox tb_modName = this.FindControl<TextBox>("tb_modName");
            NumericUpDown tb_modLoadPriority = this.FindControl<NumericUpDown>("tb_modLoadPriority");
            TextBox tb_modVersion = this.FindControl<TextBox>("tb_modVersion");
            TextBox tb_modDescription = this.FindControl<TextBox>("tb_modDescription");
            ClearFieldsCreate(tb_folderPath, tb_modName, tb_modLoadPriority, tb_modVersion, tb_modDescription);
        }

        private async void Create_OnBrowseClicked(object? sender, RoutedEventArgs e)
        {
            TextBox tb_folderPath = this.FindControl<TextBox>("tb_folderPath");
            OpenFolderDialog dialog = new();
            string? directory = await dialog.ShowAsync(this);
            if (directory == null) return;
            tb_folderPath.Text = directory;
        }

        private void ClearFieldsCreate(TextBox tb_folderPath, TextBox tb_modName, NumericUpDown tb_modLoadPriority, TextBox tb_modVersion, TextBox tb_modDescription)
        {
            tb_folderPath.Text = "";
            tb_modName.Text = "";
            tb_modLoadPriority.Text = "1";
            tb_modVersion.Text = "1.0.0";
            tb_modDescription.Text = "";
        }

        public void OnCreateClickedPublish(object sender, RoutedEventArgs e)
        {
            // check if all fields are filled out
            if(String.IsNullOrEmpty(tb_folderPathOutput.Text) || String.IsNullOrEmpty(tb_folderPathPublish.Text) || String.IsNullOrEmpty(tb_modNamePublish.Text) ||String.IsNullOrEmpty(tb_modVersionPublish.Text) || String.IsNullOrEmpty(tb_iconPathPublish.Text))
            {
                lbl_statusPublish.Content = "Error: Some fields are not filled out!";
                return;
            }

            // make folders
            if (Directory.Exists(tb_folderPathOutput.Text))
            {
                lbl_statusPublish.Content = "Error: Directory/Files already exists!";
                return;
            }

            // create temp folders n shit
            string[] x = tb_folderPathOutput.Text.Split("\\");
            string tempFolder = "";
            for (int i = 0; i < x.Length - 2; i++)
            {
                tempFolder += x[i] + "/";
            }

            Directory.CreateDirectory(tempFolder + @"/Release");
            FileSystem.CopyDirectory(tb_folderPathPublish.Text, tempFolder + @"/Release", UIOption.AllDialogs);

            // copy in actual mod, delete temp dir
            Directory.CreateDirectory(tb_folderPathPublish.Text);
            FileSystem.CopyDirectory(tempFolder + @"/Release", tb_folderPathOutput.Text +  @"/mods/" + tb_modNamePublish.Text, UIOption.AllDialogs);
            FileSystem.DeleteDirectory(tempFolder + @"/Release", UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently); 

            // copy in the readme if exists, else make one
            if(File.Exists(tb_folderPathPublish.Text + @"/README.md"))
            {
                FileSystem.CopyFile(tb_folderPathPublish.Text + @"/README.md", tb_folderPathOutput.Text + @"/README.md");
            }
            else
            {
                using (FileStream fs = File.Create(tb_folderPathOutput.Text + "/README.md"))
                {
                    string res = GetEmbeddedResourceContentManifest("NSModCreator.TEMPLATE_readme.md.txt", tb_modNamePublish.Text, tb_modVersionPublish.Text, tb_modWebsitePublish.Text, tb_modDescriptionPublish.Text, tb_modDependenciesPublish.Text);
                    byte[] info = new UTF8Encoding(true).GetBytes(res);
                    fs.Write(info, 0, info.Length);
                }
            }

            // copy the icon
            FileSystem.CopyFile(tb_iconPathPublish.Text, tb_folderPathOutput.Text + "/icon.png");

            // make manifest.json
            using (FileStream fs = File.Create(tb_folderPathOutput.Text + "/manifest.json"))
            {
                string res;
                if(tb_modDependenciesPublish.Text == "" || tb_modDependenciesPublish.Text == null) // no deps
                {
                    res = GetEmbeddedResourceContentManifest("NSModCreator.TEMPLATE_ND_manifest.json.txt", tb_modNamePublish.Text, tb_modVersionPublish.Text, tb_modWebsitePublish.Text, tb_modDescriptionPublish.Text, tb_modDependenciesPublish.Text);
                }
                else
                {
                    res = GetEmbeddedResourceContentManifest("NSModCreator.TEMPLATE_D_manifest.json.txt", tb_modNamePublish.Text, tb_modVersionPublish.Text, tb_modWebsitePublish.Text, tb_modDescriptionPublish.Text, tb_modDependenciesPublish.Text);
                }

                byte[] info = new UTF8Encoding(true).GetBytes(res);
                fs.Write(info, 0, info.Length);
                lbl_statusPublish.Content = "Release created at: " + tb_folderPathOutput.Text;
            }
        }

        public void OnClearClickedPublish(object sender, RoutedEventArgs e)
        {

        }

        private async void Publish_OnBrowseClicked(object? sender, RoutedEventArgs e)
        {
            TextBox tb_folderPathPublish = this.FindControl<TextBox>("tb_folderPathPublish");
            TextBox tb_folderPathOutput = this.FindControl<TextBox>("tb_folderPathOutput");
            OpenFolderDialog dialog = new();
            string? directory = await dialog.ShowAsync(this);
            if (directory == null) return;
            tb_folderPathPublish.Text = directory;

            if (tb_folderPathOutput.Text == "" || tb_folderPathOutput.Text == null) tb_folderPathOutput.Text = tb_folderPathPublish.Text + @"\Release";

            PreloadPublishData(tb_folderPathPublish);
        }

        private async void Publish_OnBrowseClickedIcon(object? sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            var result = await dialog.ShowAsync(this);
            if (result == null) return;
            tb_iconPathPublish.Text = result[0];
        }

        private void PreloadPublishData(TextBox tb_folderPathPublish)
        {
            using (StreamReader r = new StreamReader(tb_folderPathPublish.Text + "/mod.json"))
            {
                string json = r.ReadToEnd();
                ModJson modJson = JsonConvert.DeserializeObject<ModJson>(json);

                tb_modNamePublish.Text = modJson.Name;
                tb_modVersionPublish.Text = modJson.Version;
                tb_modDescriptionPublish.Text = modJson.Description;
            }
        }

        private async void Publish_OnBrowseClickedOutput(object? sender, RoutedEventArgs e)
        {
            TextBox tb_folderPathOutput = this.FindControl<TextBox>("tb_folderPathOutput");
            OpenFolderDialog dialog = new();
            string? directory = await dialog.ShowAsync(this);
            if (directory == null) return;
            tb_folderPathOutput.Text = directory;
        }

        private void ClearFieldsPublish()
        {

        }

        public static string GetEmbeddedResourceContentManifest(string resourceName, string modName, string version, string website, string description, string dependencies)
        {
            // handle deps
            if(dependencies != "" && dependencies != null)
            {
                string[] allDeps = dependencies.Split(",");
                string tmp = "";
                foreach(string dep in allDeps)
                {
                    tmp += "\"" + dep.Trim() + "\",";
                }

                dependencies = tmp.Substring(0, tmp.Length-1);
            }

            Assembly asm = Assembly.GetExecutingAssembly();
            Stream stream = asm.GetManifestResourceStream(resourceName);
            StreamReader source = new StreamReader(stream);
            string fileContent = source.ReadToEnd()
                .Replace("PLACEHOLDER_NAME", modName)
                .Replace("PLACEHOLDER_VERSION", version)
                .Replace("PLACEHOLDER_WEBSITE", website)
                .Replace("PLACEHOLDER_DESC", description)
                .Replace("PLACEHOLDER_DEP", dependencies);
            source.Dispose();
            stream.Dispose();
            return fileContent;
        }

        public static string GetEmbeddedResourceContent(string resourceName, string modName, string loadPriority,
            string requiredOnClient, string version, string description, string fileName, string initName)
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
