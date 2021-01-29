using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tutorial6.Services;

namespace tutorial6.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;  
        public LoggingMiddleware(RequestDelegate next) { 
            _next = next; 
        }
        public async Task InvokeAsync(HttpContext context ,IStudentsDbService service)   
        {
            if(context.Request != null)
            {
                string method = context.Request.Method;
                string path = context.Request.Path.ToString(); 
                string queryst = context.Request?.QueryString.ToString();  
                string body = "";

                using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true)) 
                {
                    body = await reader.ReadToEndAsync();
                }

                var logfile = @"C:\Users\tahas\Desktop\apbd\tutorials\tut6\tutorial6\tutorial6\requestsLog.txt";

               StreamWriter writer = File.AppendText(logfile);

                    writer.WriteLine(method);
                    writer.WriteLine(path);
                    writer.WriteLine(body);
                    writer.WriteLine(queryst);
                    writer.WriteLine("------------------------");
                    writer.Close();

                
                service.SaveLogData("data...");

            }
           
            if (_next != null) 
            {
                await _next(context);  
            }
        }
    }
}
