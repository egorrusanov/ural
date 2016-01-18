using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;

namespace Ural
{
    class Program
    {
        public static string[] serviceId;
        public static string[] deptTitle;
        public static string[] procedureId;
        public static string[] fullInfo;
        private const string Dir = "Files";
        private static readonly string DirJson = Path.Combine("Files", "JSON");
        
        static void Main(string[] args)
        {
            InitDirectory();
            //var forms = GetServiceList();
            var forms = GetForms();

            //File.AppendAllLines(Path.Combine(Dir, "forms.txt"), forms);

            //var listform = file.readalllines(path.combine(dir, "forms.txt")).distinct().tolist();
            //foreach (var form in listform)
            //{
            //    //file.writealltext(path.combine(dir, form + ".txt"), getcontent(form));
            //    file.appendalllines(path.combine(dir, "services_" + form + ".txt"), getlistservice(form));
            //}

            //foreach (var form in listform)
            //{
            //    file.delete(path.combine(dir, "status_" + form + ".txt"));
            //    var listservice = file.readalllines(path.combine(dir, "services_" + form + ".txt")).distinct().tolist();
            //    foreach (var service in listservice)
            //    {
            //        if (getlistid(service) == "привязка отсутствует")
            //            file.appendalllines(path.combine(dir, "status_" + form + ".txt"), new[] { service + "=>" + getlistid(service) }); ;
            //    }

            //}

            Console.WriteLine("Выполнено");
            Console.ReadLine();
        }

        private static void InitDirectory()
        {
            if (!Directory.Exists(Dir))
                Directory.CreateDirectory(Dir);
            if (!Directory.Exists(DirJson))
                Directory.CreateDirectory(DirJson);
        }

        private static void SaveToFile(string path, string result)
        {
            if(File.Exists(path))
                File.Delete(path);
            var stream = File.Create(path);
            stream.Close();

            File.AppendAllText(path, result);
        }

        private static string[] GetServiceList()
        {
            var path = Path.Combine(DirJson, "allServicesList.json");

            var response = Post("https://gosuslugi.volganet.ru/portal/dispatcher/rgu.searchService", "{\"reciever\":null,\"digital\":true,\"search\":\"\",\"max\":\"1024\"}");

            SaveToFile(path, response);

            var model = DeserializeServicesList(path);

            string[] services = new string[model.data.list.Count];
            string[] serviceInfo = new string[model.data.list.Count];
            for (var i = 0; i < model.data.list.Count; i++)
            {
                serviceInfo[i] = model.data.list[i].full_title + ";" + model.data.list[i].dept + ";" + model.data.list[i].level_title + "\r\n";
                File.AppendAllText(Path.Combine(Dir, "ServicesInfo.txt"), serviceInfo[i]);
            }
            return services;
        }

        private static ServicesList DeserializeServicesList(string path)
        {
            var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            var serializer = new DataContractJsonSerializer(typeof(ServicesList));
            var model = (ServicesList)serializer.ReadObject(fs);
            fs.Close();

            return model;
        }

        private static string[] GetForms()
        {
            //File.Delete(Path.Combine(Dir, "forms.txt"));
            var path = Path.Combine(DirJson, "allForms.json");

            var response = Post("https://gosuslugi.volganet.ru/portal/dispatcher/registry.list", "{\"registry\":\"form\",\"skip\":0,\"max\":\"200\",\"search\":{},\"sort\":{\"procedure\":1},\"fields\":{}}");

            SaveToFile(path, response);

            var model = DeserializeListForms(path);
            string[] formId = new string[model.data.list.Count];
            string resultFile = "AllInfo_" + DateTime.Today.Year + "_" + DateTime.Today.Month + "_" + DateTime.Today.Day + ".txt";
            File.AppendAllText(Path.Combine(Dir, resultFile), "Шаблон-Услуга@Адрес шаблона@ID Услуги@Ведомство@Код варианта услуги@Статус услуги\r\n");
            for (var i = 0;i < model.data.list.Count; i++)
            {
                formId[i] = model.data.list[i].id;
                //serviceId = new string[model.data.list[i].procedures.Count];
                //deptTitle = new string[model.data.list[i].procedures.Count];
                //procedureId = new string[model.data.list[i].procedures.Count];
                fullInfo = new string[model.data.list[i].procedures.Count];
                if (model.data.list[i].procedures.Count == 0) 
                {
                    string info = model.data.list[i].title + "@https://gosuslugi.volganet.ru/portal/personcab/form_editor.gsp?id=" + formId[i] + "&n=false@Отсутствует@Отсутствует@Отсутствует@Отсутствует\r\n";
                    File.AppendAllText(Path.Combine(Dir, resultFile), info);
                }
                else
                {
                    for (var j = 0; j < model.data.list[i].procedures.Count; j++)
                    {
                        fullInfo[j] = model.data.list[i].title + "@https://gosuslugi.volganet.ru/portal/personcab/form_editor.gsp?id=" + formId[i] + "&n=false@'" + model.data.list[i].procedures[j].serviceId + "@" + GetDeptTitle(model.data.list[i].procedures[j].serviceId) + "@'" + model.data.list[i].procedures[j].procedureId + "@" + GetListId(model.data.list[i].procedures[j].serviceId) + "\r\n";
                        //serviceId[j] = model.data.list[i].procedures[j].serviceId;
                        //deptTitle[j] = model.data.list[i].procedures[j].deptTitle;
                        //procedureId[j] = model.data.list[i].procedures[j].procedureId;
                        //GetListId(model.data.list[i].procedures[j].serviceId) + 
                        File.AppendAllText(Path.Combine(Dir, resultFile), fullInfo[j]);
                    }
                }

                
                Console.WriteLine(i + ". Выгружена форма " + model.data.list[i].id);
            }

            return formId;
        }

        public static string[] GetServiceId() { return serviceId; }

        public static string[] GetDeptTitle() { return deptTitle; }

        public static string[] GetProcedureId() { return procedureId; }

        public static string[] GetFullInfo() { return fullInfo; }

        private static string GetFormTitle(string form)
        {
            var path = Path.Combine(DirJson, "FormTitle.json");

            var response = Post("https://gosuslugi.volganet.ru/portal/dispatcher/registry.list", "{\"registry\":\"form\",\"skip\":0,\"max\":\"200\",\"search\":{},\"sort\":{\"procedure\":1},\"fields\":{}}");

            SaveToFile(path, response);

            var model = DeserializeListForms(path);
            string[] formId = new string[model.data.list.Count];
            string title = "";

            for (var i = 0; i < model.data.list.Count; i++)
            {
                if (model.data.list[i].id == form)
                    title = model.data.list[i].title;
            }

            return title;
        }

        private static string GetDeptTitle(string serviceId)
        {
            var path = Path.Combine(DirJson, "DeptTitle.json");

            var response = Post("https://gosuslugi.volganet.ru/portal/dispatcher/rgu.getService", "{\"id\":\"" + serviceId + "\"}");

            SaveToFile(path, response);

            var model = DeserializeDept(path);
            string deptTitle = model.data.r_state_structure.title;

            return deptTitle;
        }
        private static string[] GetContent(string form)
        {
            string[] content = GetFullInfo();            

            return content;
        }
        private static string[] GetListService(string form)
        {
            File.Delete(Path.Combine(Dir, "services_" + form + ".txt"));
            var path = Path.Combine(DirJson, form + ".json");

            var response = Post("https://gosuslugi.volganet.ru/portal/dispatcher/registry.get", "{\"registry\":\"form\",\"id\":\"" + form + "\"}");

            SaveToFile(path, response);

            var model = Deserialize(path);
            string[] serviceId = new string[model.data.procedures.Count];

            for (var i = 0; i < model.data.procedures.Count; i++)
            {
                serviceId[i] = model.data.procedures[i].serviceId;

            }

            return serviceId;
        }
        private static string GetListId(string service)
        {
            var path = Path.Combine(DirJson, service + ".json");

            var response = Post("https://gosuslugi.volganet.ru/portal/dispatcher/rgu.listProcedure", "{\"service\":\"" + service + "\"}");

            SaveToFile(path, response);

            var model = DeserializeListProcedure(path);
            
            string status = "";

            if (model.data.list.Count == 0)
            { status = "Привязка отсутствует"; }
            else { status = "OK"; }                    

            return status;
        }
        private static Model Deserialize(string path)
        {
            var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            var serializer = new DataContractJsonSerializer(typeof(Model));
            var model = (Model)serializer.ReadObject(fs);
            fs.Close();

            return model;
        }
        private static DeptTitle DeserializeDept(string path)
        {
            var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            var serializer = new DataContractJsonSerializer(typeof(DeptTitle));
            var model = (DeptTitle)serializer.ReadObject(fs);
            fs.Close();

            return model;
        }
        private static ListId DeserializeListProcedure(string path)
        {
            var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            var serializer = new DataContractJsonSerializer(typeof(ListId));
            var model = (ListId)serializer.ReadObject(fs);
            fs.Close();

            return model;
        }

        private static ListForms DeserializeListForms(string path)
        {
            var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            var serializer = new DataContractJsonSerializer(typeof(ListForms));
            var model = (ListForms)serializer.ReadObject(fs);
            fs.Close();

            return model;
        }  

        private static string Post(string url, string postData)
        {
            // Create a request using a URL that can receive a post. 
            WebRequest request = WebRequest.Create(url);
            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Create POST data and convert it to a byte array.
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/json";
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();
            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
            //Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            //Console.WriteLine(responseFromServer);
            // Clean up the streams.
            reader.Close();
            dataStream.Close();
            response.Close();
            return responseFromServer;
        }

        
    }
}
