using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ZP.Lib;
using ZP.Lib.Core.Values;
using ZP.Lib.Net;

namespace ZP.Lib.Web
{
    public class ZsonResult : ActionResult
    {
        public ZsonResult(object obj)
        {
            this.Value = obj;
        }

        public object Value { get; set; }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var executor = context.HttpContext.RequestServices.GetRequiredService<IActionResultExecutor<ZsonResult>>();
            return executor.ExecuteAsync(context, this);
        }
    }

    public class ZsonResult<T> : ZsonResult
    {
        public T TData => (Value as NetPackage<T, ZNetErrorEnum>).Data;

        public ZsonResult() : base(null)
        {
            var ret = ZPropertyMesh.CreateObject<NetPackage<T, ZNetErrorEnum>>();
            //ret.Data = obj;
            this.Value = ret;
        }

        public ZsonResult(T obj) : base(null)
        {
            var ret = ZPropertyMesh.CreateObject<NetPackage<T, ZNetErrorEnum>>();
            ret.Data = obj;
            this.Value = ret;
        }

        public ZsonResult(ZNetErrorEnum error) : base(null)
        {
            var ret = ZPropertyMesh.CreateObject<NetPackage<T, ZNetErrorEnum>>();
            ret.Error = error;
            ret.Data = default(T);
            this.Value = ret;
        }

        public static implicit operator ZsonResult<T>(T d)  // implicit digit to byte conversion operator
        {
            var newProp = new ZsonResult<T>(d);  // implicit conversion

            return newProp;
        }

        public static implicit operator ZsonResult<T>(ZNetErrorEnum error)  // implicit digit to byte conversion operator
        {
            var newProp = new ZsonResult<T>(error);  // implicit conversion
            //newProp.
            return newProp;
        }
    }

    public class ZsonResult<T, TError> : ZsonResult
    {
        public T TData {
            get
            {
                var ret = (Value as NetPackage<T, TError>).Data;
                if (ret == null)
                    return (Value as NetPackage<T, ZNetErrorEnum>).Data;

                return ret;
            }
                
        }
           // ?? (Value as NetPackage<T, ZNetErrorEnum>).Data);

        public ZsonResult((T obj, TError error) result) : base(null)
        {
            var ret = ZPropertyMesh.CreateObject<NetPackage<T, TError>>();
            ret.Data = result.obj;
            ret.Error = result.error;
            this.Value = ret;
        }

        public ZsonResult() : base(null)
        {
            var ret = ZPropertyMesh.CreateObject<NetPackage<T, TError>>();
            //ret.Data = default(T);
            ret.Error = default(TError);
            this.Value = ret;
        }

        public ZsonResult(T d) : base(null)
        {
            var ret = ZPropertyMesh.CreateObject<NetPackage<T, TError>>();
            ret.Data = d;
            ret.Error = default(TError);
            this.Value = ret;
        }

        public ZsonResult(ZNetErrorEnum error) : base(null)
        {
            var ret = ZPropertyMesh.CreateObject<NetPackage<T, ZNetErrorEnum>>();
            ret.Error = error;
            ret.Data = default(T);
            this.Value = ret;
        }

        public ZsonResult(TError error) : base(null)
        {
            var ret = ZPropertyMesh.CreateObject<NetPackage<T, TError>>();
            ret.Error = error;
            this.Value = ret;
        }

        public static implicit operator ZsonResult<T, TError>(T d)  // implicit digit to byte conversion operator
        {
            var newProp = new ZsonResult<T, TError>(d);  // implicit conversion

            return newProp;
        }

        public static implicit operator ZsonResult<T, TError>((T d, TError error) result)  // implicit digit to byte conversion operator
        {
            var newProp = new ZsonResult<T, TError>((result.d, result.error));  // implicit conversion

            return newProp;
        }

        public static implicit operator ZsonResult<T, TError>(TError error)  // implicit digit to byte conversion operator
        {
            var newProp = new ZsonResult<T, TError>((default(T), error));  // implicit conversion

            return newProp;
        }

        public static implicit operator ZsonResult<T, TError>(ZNetErrorEnum error)  // implicit digit to byte conversion operator
        {
            var newProp = new ZsonResult<T, TError>(error);  // implicit conversion

            return newProp;
        }
    }




    public class ZsonResultExecutor : IActionResultExecutor<ZsonResult>
    {


        private static readonly string DefaultContentType;

        private readonly IHttpResponseStreamWriterFactory _writerFactory;
        //private readonly MvcJsonOptions _options;
        //private readonly IArrayPool<char> _charPool;

        static ZsonResultExecutor()
        {
            var media = new MediaTypeHeaderValue("application/json");

            media.CharSet = Encoding.UTF8.WebName;
            ZsonResultExecutor.DefaultContentType = media.ToString();
        }

        public ZsonResultExecutor(IHttpResponseStreamWriterFactory writerFactory,
           // IOptions<MvcJsonOptions> options,
            ArrayPool<char> charPool)
        {

            if (writerFactory == null)
            {
                throw new ArgumentNullException(nameof(writerFactory));
            }


            if (charPool == null)
            {
                throw new ArgumentNullException(nameof(charPool));
            }

            _writerFactory = writerFactory;
            //_options = options.Value;
            //_charPool = new JsonArrayPool<char>(charPool);
        }

        public async Task ExecuteAsync(ActionContext context, ZsonResult result)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var response = context.HttpContext.Response;

            //not work on NetCore3.1
            //ResponseContentTypeHelper.ResolveContentTypeAndEncoding(
            //    null,
            //    response.ContentType,
            //    DefaultContentType,
            //    out var resolvedContentType,
            //    out var resolvedContentTypeEncoding);

            //response.ContentType ??
            response.ContentType =  DefaultContentType;

            //var serializerSettings = _options.SerializerSettings;

            using (var writer = _writerFactory.CreateWriter(response.Body, Encoding.UTF8))
            {
                //using (var jsonWriter = new JsonTextWriter(writer))
                //{
                //    jsonWriter.ArrayPool = _charPool;
                //    jsonWriter.CloseOutput = false;
                //    jsonWriter.AutoCompleteOnClose = false;

                //    var jsonSerializer = JsonSerializer.Create(serializerSettings);
                //    jsonSerializer.Serialize(jsonWriter, result.Value);
                //}
                
                var content =ZPropertyPrefs.ConvertToStr(result.Value);

                await writer.WriteAsync(content);

                await writer.FlushAsync();
            }
        }
    }

}
