using GameStore.Data;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace GameStore.Infrastructure
{
    [HtmlTargetElement("Game")]
    public class GameTagHelper : TagHelper
    {
        [HtmlAttributeName("game")]
        public Game Game { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            

            var content = new StringBuilder();
            content.Append("<div class='card border-2 border-dark rounded'>"); 

            content.Append("<img class='card-img-top img-fluid'"); 
            content.Append($"src='{Game.CoverImageUrl}'");
            content.Append($"alt='{Game.Title}'");
            content.Append("style='object-fit: cover; height: 400px;' />");

            content.Append("<div class='card-body bg-dark text-white' style='height:100px'> "); 
            content.Append($"<h5 class='card-title'>{Game.Title}</h5>");
            content.Append("</div>"); 

            content.Append("</div>");
            output.Content.SetHtmlContent(content.ToString());
        }
    }
}
