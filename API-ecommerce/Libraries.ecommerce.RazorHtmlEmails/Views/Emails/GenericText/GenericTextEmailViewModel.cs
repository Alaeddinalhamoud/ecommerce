using ecommerce.Data;
using ecommerce.Data.MVData;
using System.Collections.Generic;


namespace Libraries.ecommerce.RazorHtmlEmails.Views.Emails.GenericText
{
    public class GenericTextEmailViewModel
    {
        public GenericTextEmailViewModel(string text)
        {
            Text = text;
        }

        public string Text { get; set; }
    }
}
