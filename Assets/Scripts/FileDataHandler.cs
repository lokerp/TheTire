using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class FileDataHandler
{
    private string filePath = "";
    private string fileName = "";

    public FileDataHandler(string filePath, string fileName)
    {
        this.filePath = filePath;
        this.fileName = fileName;
    }

    public void Save(Database database)
    {
        string fullPath = Path.Combine(filePath, fileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            string json = JsonConvert.SerializeObject(database, Formatting.Indented);

            using (FileStream fstream = new FileStream(fullPath, FileMode.Create))
            {
                using(StreamWriter swriter = new(fstream))
                {
                    swriter.Write(json);
                }
            }
        }

        catch (System.Exception e) 
        {
            Debug.LogException(e);
        }
    }

    public Database Load()
    {
        string fullPath = Path.Combine(filePath, fileName);

        Database database = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string data = "";
                using (FileStream fstream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader swriter = new(fstream))
                    {
                        data = swriter.ReadToEnd();
                    }
                }

                database = JsonConvert.DeserializeObject<Database>(data);
            }

            catch(System.Exception e)
            {
                Debug.LogException(e);
            }
        }
        return database;
    }
}
