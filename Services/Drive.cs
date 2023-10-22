using Google.Apis.Drive.v3;
using File = Google.Apis.Drive.v3.Data.File;

namespace gsuit_api_maui
{

    public class Drive
    {
        public Drive()
        {
        }

        public List<File> GetDriveFiles(string[] keywords, DriveService driveService)
        {
            List<File> Files = new List<File>();
            foreach (string keyword in keywords)
            {
                try
                {
                    FilesResource.ListRequest files = driveService.Files.List();
                    files.Fields = "nextPageToken,files(id,name,mimeType,webViewLink,permissions)";
                    files.Corpora = "user";
                    files.IncludeItemsFromAllDrives = true;
                    files.IncludeTeamDriveItems = true;
                    files.SupportsAllDrives = true;
                    files.Q = $"fullText contains '\"{keyword}\"'";
                    List<File> result = (List<File>)files.Execute().Files;
                    Files.AddRange(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
            return Files;
        }
    }
}