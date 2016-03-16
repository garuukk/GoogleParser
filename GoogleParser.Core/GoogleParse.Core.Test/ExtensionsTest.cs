using System;
using System.Net;
using GoogleParser.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoogleParse.Core.Test
{
    [TestClass]
    public class ExtensionsTest
    {
        [TestMethod]
        public void Test()
        {
            string PLAY_PREFS = "PLAY_PREFS=CjwIABI4CgJSVRDE3bDtsiooxN2w7bIqOiQ0OGJlYTMyZS1lMTgyLTQ0MGItYTgwYS00ZDNiNmJjOWRhNzA:S:ANO1ljJCpFTvsNvTgg";
            string path = "Path=/";
            string secure = "Secure";

            string testValue = "PLAY_PREFS=CjwIABI4CgJSVRCjwZTKsSooo8GUyrEqOiQ3YmNmOWNmOC00MzgzLTQ5MTYtYTgzMC0yZmY5NTY4ZWY0ZTA:S:ANO1ljLK3p3vmQ8pKQ; Path=/; Secure; HttpOnly NID=76=D7sQZI2MvMIGCmCgL8Ub9sy-y5Ydz9fBq0F2dTqHmyAbaBkL4xLa_8JGdbcuNCPa9MNIN9mQ7oTIen4yjrYrC6UCLvaccv4PneE-QZKbAb8A0eAfY1aBkZmGxcl5SqhY;Domain=.google.com;Path=/;Expires=Fri, 26-Aug-2016 16:31:14 GMT;HttpOnly";

            CookieContainer cookieContainer = new CookieContainer();

            var uri = new Uri("https://play.google.com");
            cookieContainer.SetCookies(uri, testValue);

            var coo = cookieContainer.GetCookies(uri);
            var c = coo;


            var result = Extentions.ParseKeyValuePair(testValue, ';', '=');

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 4);

// то что получили при первом запросе
//Cache-Control	no-cache, no-store, max-age=0, must-revalidate
//Content-Encoding gzip
//Content-Type text/html; charset=utf-8
//Date Thu, 25 Feb 2016 16:31:14 GMT
//Expires Fri, 01 Jan 1990 00:00:00 GMT
//Pragma no-cache
//Server GSE
//Set-Cookie PLAY_PREFS=CjwIABI4CgJSVRCjwZTKsSooo8GUyrEqOiQ3YmNmOWNmOC00MzgzLTQ5MTYtYTgzMC0yZmY5NTY4ZWY0ZTA:S:ANO1ljLK3p3vmQ8pKQ; Path=/; Secure; HttpOnlyNID=76=D7sQZI2MvMIGCmCgL8Ub9sy-y5Ydz9fBq0F2dTqHmyAbaBkL4xLa_8JGdbcuNCPa9MNIN9mQ7oTIen4yjrYrC6UCLvaccv4PneE-QZKbAb8A0eAfY1aBkZmGxcl5SqhY;Domain=.google.com;Path=/;Expires=Fri, 26-Aug-2016 16:31:14 GMT;HttpOnly
//X-Content-Type-Options nosniff
//X-Firefox-Spdy h2
//X-Frame-Options SAMEORIGIN
//X-XSS-Protection 1; mode=block
//p3p CP="This is not a P3P policy! See https://support.google.com/accounts/answer/151657?hl=en for more info
//."


// то что отправляется при POST запросе

//Accept text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8
//Accept-Encoding gzip, deflate
//Accept-Language en-US,en;q=0.5
//Connection keep-alive
//Content-Length 78
//Content-Type application/x-www-form-urlencoded;charset=utf-8
//    Cookie PLAY_PREFS=CjwIABI4CgJSVRCjwZTKsSooo8GUyrEqOiQ3YmNmOWNmOC00MzgzLTQ5MTYtYTgzMC0yZmY5NTY4ZWY0ZTA:S:ANO1ljLK3p3vmQ8pKQ ; NID=76=D7sQZI2MvMIGCmCgL8Ub9sy-y5Ydz9fBq0F2dTqHmyAbaBkL4xLa_8JGdbcuNCPa9MNIN9mQ7oTIen4yjrYrC6UCLvaccv4PneE-QZKbAb8A0eAfY1aBkZmGxcl5SqhY ; S=billing-ui-v3=VNoGQhByIm4sFjp4Z0qx0w:billing-ui-v3-efe=VNoGQhByIm4sFjp4Z0qx0w
//Host play.google.com
//Referer https://play.google.com/store
//User-Agent Mozilla/5.0 (Windows NT 6.1; WOW64; rv:43.0) Gecko/20100101 Firefox/43.0
        
// POST параметры

//ipf=1
//num=5
//numChildren=10
//pagTok=CAUQBQ==:S:ANO1ljKVHaE
//start=5
//xhr=1


//billing-ui-v3=QKR6U4SLoJR9n0UjgFiY7A:billing-ui-v3-efe=QKR6U4SLoJR9n0UjgFiY7A
//billing-ui-v3=VNoGQhByIm4sFjp4Z0qx0w:billing-ui-v3-efe=VNoGQhByIm4sFjp4Z0qx0w

        }
    }
}
