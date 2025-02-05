using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using Volo.Abp;
using System.Threading;

namespace ApiRepositorio.Class
{
    public class ExceptionHandlingAttribute : ExceptionFilterAttribute
    {

        //public override async Task OnExceptionAsync(HttpActionExecutedContext context, CancellationToken cancellationToken)
        //{
        //    var stream = await context.Request.Content.ReadAsStreamAsync();
        //    stream.Position = 0;
        //    using (var reader = new StreamReader(stream))
        //    {
        //        var requestString = reader.ReadToEnd();
        //    }
        //}


        public override void OnException(HttpActionExecutedContext context)
        {


            //var stream = context.Request.Content.ReadAsStringAsync();
           
            //using (var reader = new StreamReader(stream))
            //{
            //    var requestString = reader.ReadToEnd();
            //}


            if (context.Exception is BusinessException)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(context.Exception.Message),
                    ReasonPhrase = "Exception"
                });

            }

            //Log Critical errors
            Debug.WriteLine(context.Exception);

            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(context.Exception.Message.ToString()),
                ReasonPhrase = "Exception"
            });
        }
    }
}