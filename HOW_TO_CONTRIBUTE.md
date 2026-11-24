# Restore Aquatic Essential Source

Welcome to Restore Aquatic. Thusfar, builds do not exist. If you are looking to beta-test, you will not find that here.

# Developers / Maintainers

## Preliminary Setup

If you are a developer / maintainer, then ensure that you have done the following.

1. Installed the latest Unity Hub Version.
2. Created a new Unity "Universal 3D" project locally on your computer.
3. Set the Project Editor Version to 6000.6.2f1
    3a. This is important because older versions might now have some of the
        version control friendly defaults we want.
4. Named your project "Restore Aquatic"
5. Clicked "Create Project"

## How do I connect to the GitHub?

Ensure that you have requested contributor permissions. You will need these when you have changes to push. For now, you can setup the environment without permissions.

1. `cd` into the directory that your Unity Project is in.
   1a. It MUST be the project root directory that directly contains directories:
	I.   Assets
	II.  Logs
	III. ProjectSettings
	etc...

2. Run the following to initialize your repository, and pull the remote changes to overwrite those defaults in your local project directory.

```
git init
git remote add origin https://github.com/SkylerAndersen/Restore-Aquatic.git
git fetch
git checkout -f -B main origin/main
```

3. Test what was effectively a `pull --force` command. Do you see logs for commits you didn't make?
   3a. Try: `git log` to check these logs

If everything worked, you are all set up!
