using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using UtahCrashesCracked.Models.ViewModels;

namespace UtahCrashesCracked.Infrastructure
{
    [HtmlTargetElement("div", Attributes = "page-model")]
    public class PaginationTagHelper : TagHelper
    {
        //Dynamically create page links for us

        private IUrlHelperFactory uhf;

        public PaginationTagHelper(IUrlHelperFactory temp)
        {
            uhf = temp;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext vc { get; set; }

        // Separate from ViewContext
        public PageInfo PageModel { get; set; }
        public string PageAction { get; set; }
        public string PageClass { get; set; }
        public bool PageClassesEnabled { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected { get; set; }

        public override void Process(TagHelperContext thc, TagHelperOutput tho)
        {
            IUrlHelper uh = uhf.GetUrlHelper(vc);

            TagBuilder final = new TagBuilder("div");

            TagBuilder tb = new TagBuilder("a");

            tb.Attributes["href"] = uh.Action(PageAction, new { pageNum = PageModel.CurrentPage - 1 });

            if (PageClassesEnabled)
            {
                tb.AddCssClass(PageClass);
                tb.AddCssClass(PageClassSelected);
            }

            tb.InnerHtml.Append("<<Previous");

            tb.InnerHtml.Append("\t");

            final.InnerHtml.AppendHtml(tb);            

            for (int i = 1; i <= PageModel.TotalPages; i++)
            {

                if (i <= PageModel.CurrentPage + 3 && i >= PageModel.CurrentPage - 3)
                {
                    tb = new TagBuilder("a");

                    tb.Attributes["href"] = uh.Action(PageAction, new { pageNum = i });

                    if (PageClassesEnabled)
                    {
                        tb.AddCssClass(PageClass);
                        tb.AddCssClass(i == PageModel.CurrentPage ? PageClassSelected : PageClassNormal);
                    }
                    tb.AddCssClass(PageClass);
                    tb.InnerHtml.Append(i.ToString());
                    tb.InnerHtml.Append("\t");

                    final.InnerHtml.AppendHtml(tb);
                }
            }

            tb = new TagBuilder("a");

            tb.Attributes["href"] = uh.Action(PageAction, new { pageNum = PageModel.CurrentPage + 1 });

            if (PageClassesEnabled)
            {
                tb.AddCssClass(PageClass);
                tb.AddCssClass(PageClassSelected);
            }

            tb.InnerHtml.Append("Next>>");

            tb.InnerHtml.Append("\t");

            final.InnerHtml.AppendHtml(tb);         

            tho.Content.AppendHtml(final.InnerHtml);
        }
    }
}
