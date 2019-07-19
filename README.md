# Selenium C# Lambda

How to use Selenium (C#) Webdriver with Amazon Web Services (AWS) Lambda.

## Pre-requisite

* AWS account
    * IAM user with S3 access
    * S3 Bucket
* Visual Studio with AWS Toolkit installed

## Getting started

Download the HeadlessSeleniumSetup.zip file and upload the zip file (do not extract) to your S3 bucket.

Download the Selenium-CSharp-Lambda solution and open in visual studio. 

Edit the following code to include the credentials, region endpoint (e.g. RegionEndpoint.USEast1) and bucket name for your chosen S3 bucket

```csharp
new AmazonS3Client("[Access key ID]", "[Secret access key]", "[RegionEndpoint]"));

fileTransferUtility.Download("/tmp/HeadlessSeleniumSetup.zip", "[Bucket]", "HeadlessSeleniumSetup.zip");
```

Once you have updated this you can now upload your lambda to AWS. To do this right click on the solution and select 'Publish to AWS Lambda...'

Populate the account profile, region and function name then click next. Now select an existing role or create a new one then click upload.

## Running a test

All you need to do now is invoke the lambda. You can see the results of the selenium test in the cloudwatch logs. If the test has executed successfully you will see:

```bash
Driver title is: Google
```

## Test runner

[TestRunner](https://github.com/ideagenplc/Selenium-CSharp-Lambda/tree/master/Selenium-CSharp-TestRunner "TestRunner")

## References

The following references all played a part in helping me get this working.

[How to get headless Chrome running on AWS Lambda](https://medium.com/@marco.luethy/running-headless-chrome-on-aws-lambda-fa82ad33a9eb "How to get headless Chrome running on AWS Lambda")

[serverless-chrome](https://github.com/adieuadieu/serverless-chrome "serverless-chrome")

[Blackboard - lambda-selenium](https://github.com/blackboard/lambda-selenium "lambda-selenium")

[serverless-selenium-cs](https://github.com/Scott-Meyer/serverless-selenium-cs "serverless-selenium-cs")



## License
[MIT](https://choosealicense.com/licenses/mit/)
