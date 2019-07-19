# Selenium C# Lambda Test Runner

Test suite runner for use with Selenium (C#) Webdriver with Amazon Web Services (AWS) Lambda.

## Pre-requisite

* AWS account
    * IAM user with S3 access
    * S3 Bucket
* Visual Studio with AWS Toolkit installed

## Getting started

Upload the HeadlessSeleniumSetup.zip file file (do not extract) to your S3 bucket.

Upload the Sample.zip file (do not extract) to your S3 bucket.

Open the Trigger solution in visual studio. 

Edit the following code to include the credentials, region endpoint (e.g. RegionEndpoint.USEast1) and bucket name for your chosen S3 bucket

```csharp
using (AmazonLambdaClient client = new AmazonLambdaClient("[RegionEndpoint]"))

new AmazonS3Client("[Access key ID]", "[Secret access key]", "[RegionEndpoint]"));

fileTransferUtility.Download("/tmp/" + TestSuite + ".zip", "[Bucket]", TestSuite + ".zip");
```

Once you have updated this you can now upload your lambda to AWS. To do this right click on the solution and select 'Publish to AWS Lambda...'

Populate the account profile and region then click next. Now select an existing role or create a new one then click upload.

Repeat the same steps for the Runner solution editing the following code

```csharp
new AmazonS3Client("[Access key ID]", "[Secret access key]", "[RegionEndpoint]"));

fileTransferUtility.Download("/tmp/" + testSuite + ".zip", "[Bucket]", testSuite + ".zip"); ;
fileTransferUtility.Download("/tmp/HeadlessSeleniumSetup.zip", "[Bucket]", "HeadlessSeleniumSetup.zip");
```

## Running a test suite

After both the trigger and runner have been published to AWS you can run a test suite by invoking the trigger lambda passing in the name of the test suite you wish to run, in this case pass in "Sample". You can see the results of the test suite in the cloudwatch logs. If the suite has executed successfully you will see multiple logs for the runner, under each log you will see the following:

```bash
Driver title is: [web page title]
```

## Notes

The trigger builds the test suite by using the nunit [Test] attribute and looks for "Tests" in the class namespace. If you want to use your own test suite you will need to build your suite using nunit and ensure your tests include "Tests" in the namespace.  

This is a simple example to get you started. You can change the way the suite is built any way you like. Once you are happy with your test suite you will need to zip the contents of the bin directory and upload it to your S3 bucket, then you can invoke it by passing the name of the suite in to the trigger.

## Improvements

These examples use headless chrome which isn't perfect. You will come accross issues if your app uses iframes etc. Luckily the guys at blackboard have provided a solution to use full chrome with lambdas. The following references will get you started:

[Blackboard - lambda-selenium Chrome Binary](https://github.com/blackboard/lambda-selenium/issues/44 "Blackboard - lambda-selenium Chrome Binary") 

[aws-lambda-xvfb](https://github.com/nisaacson/aws-lambda-xvfb "aws-lambda-xvfb")

## References

The following references all played a part in helping me get this working.

[How to get headless Chrome running on AWS Lambda](https://medium.com/@marco.luethy/running-headless-chrome-on-aws-lambda-fa82ad33a9eb "How to get headless Chrome running on AWS Lambda")

[serverless-chrome](https://github.com/adieuadieu/serverless-chrome "serverless-chrome")

[Blackboard - lambda-selenium](https://github.com/blackboard/lambda-selenium "lambda-selenium")

[serverless-selenium-cs](https://github.com/Scott-Meyer/serverless-selenium-cs "serverless-selenium-cs")



## License
[MIT](https://choosealicense.com/licenses/mit/)
