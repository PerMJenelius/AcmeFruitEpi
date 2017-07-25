using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace FruitCorp.Web.Models.Pages
{
    [ContentType(DisplayName = "StartPage", GUID = "0686d430-a8ca-420a-87bf-f0581f11dcf5", Description = "The Starting Page of the site")]
    public class StartPage : PageData
    {
        /*
                [CultureSpecific]
                [Display(
                    Name = "Main body",
                    Description = "The main body will be shown in the main content area of the page, using the XHTML-editor you can insert for example text, images and tables.",
                    GroupName = SystemTabNames.Content,
                    Order = 1)]
                public virtual XhtmlString MainBody { get; set; }
         */
    }
}