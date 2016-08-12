using System.Collections.Generic;
using System.Linq;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Ruby.Internal;

namespace Ruby.Movements
{
    internal class Drive : Reflex
    {
        static DriveService service;
        static string CLIENT_ID;
        static string CLIENT_SECRET;
        static string Username;

        FileList filesOnDrive;
        FileList foldersOnDrive;

        Dictionary<string, List<string>> fileTree;
        Dictionary<string, string> names;

        List<DriveFolder> folders;
        List<DriveDoc> documents;

        DriveFolder root;

        public Drive() : base()
        {
            CLIENT_ID = Core.FilePaths.GoogleAPIClient;
            CLIENT_SECRET = Core.FilePaths.GoogleAPISecret;
            Username = Core.FilePaths.GoogleUsername;

            filesOnDrive = null;
            foldersOnDrive = null;

            folders = new List<DriveFolder>();
            documents = new List<DriveDoc>();

            root = new DriveFolder("0ACzWi1YWnW6oUk9PVA", "Root", new List<ParentReference>());
            Name = "drive";
            ID = "x";
        }

        public override void Initialize()
        {
            //service = new DriveService();

            //string[] scopes = new string[] { DriveService.Scope.Drive, DriveService.Scope.DriveFile};

            //UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync
            //(
            //    new ClientSecrets { ClientId = CLIENT_ID, ClientSecret = CLIENT_SECRET }, 
            //    scopes, 
            //    Username,
            //    CancellationToken.None, 
            //    new FileDataStore("Daimto.GoogleDrive.Auth.Store")
            //).Result;

            //service = new DriveService(new BaseClientService.Initializer()
            //{
            //    HttpClientInitializer = credential,
            //    ApplicationName = "Drive API Sample",
            //});

            //Task getFiles = Task.Factory.StartNew(() => GetFiles());
        }

        //public override void SetPrimarySpecs()
        //{
        //    PrimarySpecs = new Dictionary<string, string[]>();
        //}

        //public override void SetSecondarySpecs()
        //{
        //    SecondarySpecs = new Dictionary<string, string[]>();
        //}

        //public override void SetDefaultSpec()
        //{
        //    DefaultSpec = string.Empty;
        //}

        //public override void GatherValues() { }

        private Google.Apis.Drive.v2.Data.File GetFileByID(string id)
        {
            return filesOnDrive.Items.First(f => f.Id.Equals(id));
        }

        public void GetFiles()
        {
            FilesResource.ListRequest requestFolders = service.Files.List();
            requestFolders.Q = "mimeType='application/vnd.google-apps.folder' and trashed=false";
            requestFolders.MaxResults = 1000;
            foldersOnDrive = requestFolders.Execute();

            folders.Add(root);

            foreach (Google.Apis.Drive.v2.Data.File folder in foldersOnDrive.Items.Where(fl => fl.Parents.Count > 0))
            {
                folders.Add(new DriveFolder(folder.Id, folder.Title, folder.Parents));
            }

            GetChildren(root);

            FilesResource.ListRequest requestFiles = service.Files.List();
            requestFiles.Q = "mimeType!='application/vnd.google-apps.folder' and trashed=false";
            requestFiles.MaxResults = 1000;
            filesOnDrive = requestFiles.Execute();

            foreach (Google.Apis.Drive.v2.Data.File doc in filesOnDrive.Items.Where(fl => fl.Parents.Count > 0))
            {
                DriveFolder parentFolder = folders.FirstOrDefault(folder => folder.ID.Equals(doc.Parents.First().Id));

                if(parentFolder != null)
                {
                    parentFolder.Children.Add(new DriveDoc(doc.Id, doc.Title, doc.Parents));
                }
            }
        }

        private void GetChildren(DriveFolder folder)
        {
            string id = folder.ID;
            string parentID = folder.ParentID;
            DriveFolder parent = folders.FirstOrDefault(fold => fold.ID.Equals(parentID));

            IEnumerable<DriveFolder> children = folders.Where(fl => fl.ParentID.Equals(id));

            foreach (DriveFolder fold in children)
            {
                GetChildren(fold);
            }

            if(parent != null)
            {
                parent.Children.Add(folder);
            }
        }

        //public void DisplayFiles()
        //{
        //    TreeViewItem rt = new TreeViewItem();
        //    AddToTree(ref rt, root);
        //    Hub.GUI.window.fileTreeView.Items.Add(rt);
        //    Hub.GUI.window.Height = 500;
        //}

        //private void AddToTree(ref TreeViewItem parent, DriveFolder fold)
        //{
        //    TreeViewItem current = CreateTreeItem(fold);

        //    IEnumerable<DriveFolder> folders = fold.Children.OfType<DriveFolder>();
        //    IEnumerable<DriveDoc> documents = fold.Children.OfType<DriveDoc>();

        //    foreach (DriveFolder folder in folders)
        //    {
        //        AddToTree(ref current, folder);
        //        current.Items.Add(CreateTreeItem(folder));
        //    }

        //    foreach (DriveDoc document in documents)
        //    {
        //        current.Items.Add(CreateTreeItem(document));
        //    }

        //    parent.Items.Add(current);
        //}

        //private TreeViewItem CreateTreeItem(DriveElement elem)
        //{
        //    TreeViewItem item = new TreeViewItem();
        //    item.Header = elem.Title;
        //    item.Tag = elem.ID;
        //    item.FontWeight = FontWeights.Normal;
        //    item.Expanded += new RoutedEventHandler(folder_Expanded);

        //    return item;
        //}

        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{
            
        //}

        //void folder_Expanded(object sender, RoutedEventArgs e)
        //{

        //}

        //public static string DownloadFile(Google.Apis.Drive.v2.Data.File file)
        //{
        //    try
        //    {
        //        var x = service.HttpClient.GetByteArrayAsync(file.ExportLinks["text/plain"]);
        //        byte[] arrBytes = x.Result;
        //        return System.Text.Encoding.Default.GetString(arrBytes);
        //    }
        //    catch (Exception e)
        //    {
        //        return null;
        //    }
        //}

        //public void Upload_File(string file)
        //{
        //    Utilities.Drive_Utility.uploadFile(service, file);
        //}
    }
}
