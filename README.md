# BuildNumberGenerator
Generates unique build numbers for based on branch and date.

The goal here is to produce internal build numbers for other projects. The format should be

> BranchName.YYYYMMDD.X

BranchName is the name of the branch.
YYYYMMDD is simply the date.
X is the current build number. This increments for every build in the branch and resets to 1 for the first build of the next day.

For example if I have built my test branch 3 times today the latest build would be

> test.20220706.3

If I then build merge that branch and build main I get

> main.20220706.1

The ideal is to keep the build numbers small and easy to identify. As of creating this project there few way to generate a unique build number in GitHub actions. $RUNNUMBER is the best option, but this continuouly increments. It is easier if I know there is a issue with the .2 build from today I can easily identify that.
