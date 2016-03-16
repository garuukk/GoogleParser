using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Svyaznoy.Core.Web.View
{
    public static class LinkHelper
    {
        /// <summary>
        /// Creates asyncronos post-request with confirmation/success dialog with redirect/refresh. Depends on jquery.blockUI
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="actionUrl"></param>
        /// <param name="linkLabel"></param>
        /// <param name="confirmationMessage"></param>
        /// <param name="postParams"></param>
        /// <param name="cssClass"></param>
        /// <param name="successRedirectUrl"></param>
        /// <param name="ancorLink">if true - then base tag is A else SPAN</param>
        /// <param name="progressMessage"></param>
        /// <param name="blockUI"></param>
        /// <param name="onErrorBeforeDialogJsScript"></param>
        /// <param name="onErrorInDialogJsScript"></param>
        /// <param name="onSuccessBeforeRedirectJsScript"></param>
        /// <param name="onPostJsScript"></param>
        /// <returns></returns>
        public static MvcHtmlString ProcessLink(this HtmlHelper helper,
                                        string actionUrl,
                                        string linkLabel,
                                        string confirmationMessage = null,
                                        Dictionary<string, object> postParams = null,
                                        string cssClass = null,
                                        string successRedirectUrl = null,
                                        bool ancorLink = true,
                                        string progressMessage = null,
                                        bool blockUI = true,
                                        string onErrorBeforeDialogJsScript = null,
                                        string onErrorInDialogJsScript = null,
                                        string onSuccessBeforeRedirectJsScript = null,
                                        string onPostJsScript = null,
                                        string toolTip = null)
        {
            var guid = Guid.NewGuid().ToString("N");
            successRedirectUrl = (successRedirectUrl ?? "").Trim();
            confirmationMessage = confirmationMessage.ToHtmlEmbeddableString();
            toolTip = toolTip.ToJavaScriptString();
            progressMessage = (progressMessage ?? "").Replace("'", "&apos;").Replace("\n", "<br />").Trim();
            progressMessage = string.IsNullOrWhiteSpace(progressMessage) ? "null" : "'" + progressMessage + "'";

            cssClass = (cssClass ?? "").Trim();
            linkLabel = (linkLabel ?? "").Trim();

            string postParamsString = null;
            if (postParams == null || postParams.Count == 0)
                postParamsString = "null";
            else
            {
                postParamsString += "{\n";
                int i = 0;
                foreach (var pair in postParams)
                {
                    if (!string.IsNullOrWhiteSpace(pair.Key))
                    {
                        if (i > 0)
                            postParamsString += ", ";
                        postParamsString += "   " + pair.Key + ": '" + (pair.Value ?? "").ToString() + "'";
                        i++;
                    }
                }
                postParamsString += "\n}";
            }
            string tag;

            if (ancorLink)
            {
                tag = "<a id='actionLink$=GUID$$' href='#' class='js-ui-tooltip " + cssClass + @"' title='" + toolTip + "'>" + linkLabel + @"</a>";
            }
            else
            {
                tag = "<span id='actionLink$=GUID$$' class='js-ui-tooltip " + cssClass + @"' title='" + toolTip + "'>" + linkLabel + @"</span>";
            }

            

            var script = tag+
                         @"
<div id='actionConfirmationDialog$=GUID$$'>
    <p>" + confirmationMessage + @"</p>
</div>
<div id='actionResultDialog$=GUID$$'>
    <p><span id='actionResultLabel$=GUID$$'></span> </p>
</div>
<script>
    $(function () {
        var postForm = function() {
                        
            $.post(
                '" + actionUrl + @"',
                " + postParamsString + @",
                function (data) {

                    " + (blockUI ? @"$.unblockUI();" : "") + @"

                    $('#actionResultLabel$=GUID$$').html(data.Message);
                    if (data.HasError) {
                        " + onErrorBeforeDialogJsScript + @"
                        $('#actionResultDialog$=GUID$$').dialog({
                            resizable: false,
                            draggable: false,
                            modal: true,
                            buttons: {
                                'ОК': function() {
                                    " + onErrorInDialogJsScript + @"
                                    $(this).dialog('close');
                                },
                            }
                        });
                    }
                    else if (data.Message != null && data.Message != '') {
                        $('#actionResultDialog$=GUID$$').dialog({
                            resizable: false,
                            draggable: false,
                            minHeight: 200,
                            modal: true,
                            buttons: {
                                'ОК': redirectToUrl()
                            }
                        });
                    } else {
                        redirectToUrl();
                    }
                });
                " + onPostJsScript + @"

                " + (blockUI? 
                @"$.blockUI({ 
                            message: " + progressMessage + @"
                         });":"") + @"
        };

        var redirectToUrl = function() {
            " + onSuccessBeforeRedirectJsScript + @"
            if ('" + successRedirectUrl + @"' == '')
                location.reload();
            else
                window.location ='" + successRedirectUrl + @"';
        };
                    
        $('#actionResultDialog$=GUID$$').hide();
                           

        $('#actionConfirmationDialog$=GUID$$').dialog({
            autoOpen: false,
            resizable: false,
            draggable: false,
            minHeight: 200,
            modal: true,
            buttons: {
                'ОК': function() {
                    $(this).dialog('close');
                    postForm();
                },
                'Отмена': function() {
                    $(this).dialog('close');
                }
            }
        });

        
        $('#actionProgressDialog$=GUID$$').dialog({
            autoOpen: false,
            resizable: false,
            draggable: false,
            modal: true,
        });                   

        $('#actionLink$=GUID$$').click(function() {
            if ('" + confirmationMessage.Length + @"' != '0') {
                $('#actionConfirmationDialog$=GUID$$').dialog('open');
            } else {
                postForm();
            }

            return false;
        });
    })
</script>
";
            script = script.Replace("$=GUID$$", guid);

            return new MvcHtmlString(script);
        }

        /// <summary>
        /// Creates link-like text (hide link from search engine bots)
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="url"></param>
        /// <param name="text"></param>
        /// <param name="cssClass"></param>
        /// <returns></returns>
        public static MvcHtmlString JsActiveLink(this HtmlHelper helper, string url, string text, string cssClass)
        {
            var html =
@"
<span id='$=GUID$$' class='" + cssClass + @"'>
    " + text + @"
</span>
<script>
    $(function () {
        $('#$=GUID$$').click(function() {
            window.location = '" + url + @"';
        });
    });
</script>
";
            html = html.Replace("$=GUID$$", Guid.NewGuid().ToString());

            return new MvcHtmlString(html);
        }
    }
}
