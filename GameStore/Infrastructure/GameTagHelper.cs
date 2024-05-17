using GameStore.Data;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace GameStore.Infrastructure
{
    [HtmlTargetElement("Game")]
    public class GameTagHelper : TagHelper
    {
        [HtmlAttributeName("game")]
        public required Game Game { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            

            var content = new StringBuilder();
            
            content.Append("<div class='card border-2 border-dark rounded zoom'>"); 

            content.Append("<img class='card-img-top img-fluid'"); 
            content.Append($"src='{Game.CoverImageUrl}'");
            content.Append($"alt='{Game.Title}'");
            content.Append("style='object-fit: cover; height: 400px; color:black;' />");

            content.Append("<div class='card-body bg-dark text-white' style='height:100px'> "); 
            content.Append($"<h5 class='card-title'>{Game.Title}</h5>");
            content.Append("<a href='/Games/Details/" + Game.Id + "'class='stretched-link'></a>");
            content.Append("</div>"); 

            content.Append("</div>");
            output.Content.SetHtmlContent(content.ToString());
        }
    }
}
