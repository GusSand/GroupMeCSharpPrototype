GroupMeCSharpPrototype
======================

Windows Phone C# prototype 

A Simple Windows Phone project that shows how to get the inbox for a GroupMe user. 

Notes:

- It uses HttpClient from the System.Net and Newtonsoft.Json. You might need to install Newtonsoft using the NuGet package Manager. 


- This won't work until you use your own token, or implement a signon mechanism. Once you have your GroupMe Token just 
replace the XXXX in this line in SampleDataSource.cs. 


                // TODO: add your token here
                HttpResponseMessage response = await client.GetAsync("inbox?token=XXXX");
