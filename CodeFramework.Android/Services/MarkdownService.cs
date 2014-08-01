using System;
using CodeFramework.Core.Services;

namespace CodeFramework.Android.Services
{
	public class MarkdownService : IMarkdownService
    {

		public MarkdownService()
		{
//			_ctx = new JSContext(_vm);
//			var script = System.IO.File.ReadAllText("Markdown/marked.js", System.Text.Encoding.UTF8);
//			_ctx.EvaluateScript(script);
//			_val = _ctx[new NSString("marked")];
		}

		public string Convert(string c)
		{
//			if (string.IsNullOrEmpty(c))
//				return string.Empty;
//			return _val.Call(JSValue.From(c, _ctx)).ToString();
		    throw new NotImplementedException();
		}
    }
}

