# GitHub Guide

This is how we share work on Waystation. The steps use [GitHub Desktop](https://desktop.github.com/). If you'd rather use a terminal, the commands are at the bottom.

For some context, we do our day-to-day work on `dev`. You start your branch from there, and when the work is ready you open a pull request back into `dev`. We keep `main` for releases.

Branch names and the prefixes we expect are in [CONTRIBUTION.md](CONTRIBUTION.md).

The repository is https://github.com/mbajaman/Waystation

## Before you start

1. Install GitHub Desktop from https://desktop.github.com/.
2. Open the app and sign in to GitHub.
   - On Windows: `File → Options → Accounts → Sign in`
   - On Mac: `GitHub Desktop → Settings → Accounts → Sign in`
3. Use the GitHub account that has access to Waystation. If the clone fails with a permissions error, tell us and we'll add you.

## 1. Clone the repository

You only do this once. GitHub Desktop suggests `Documents/GitHub`, and that folder works well. Put the project somewhere that isn't already inside another copy of Waystation.

1. In GitHub Desktop, choose `File → Clone repository`.
2. Open the `GitHub.com` tab and select `mbajaman/Waystation`. If it isn't in the list, open the `URL` tab and paste `https://github.com/mbajaman/Waystation.git`.
3. Choose the local folder.
4. Click `Clone`.
5. Click `Current branch` and select `dev`.

A fresh clone often lands on `main`. Switch to `dev` before you create a branch.

## 2. Create a branch

Use a new branch for each piece of work. Keep the name short, lowercase, and hyphenated, with a prefix from the contribution guide.

```
feat/planet-landing
fix/checkpoint-respawn
```

1. Click `Current branch` and select `dev`.
2. Click `Fetch origin`. If the button changes to `Pull origin`, click that too, so your `dev` matches GitHub.
3. Choose `Branch → New branch`.
4. Type the branch name. Leave `Create branch based on` set to `dev`.
5. Click `Create branch`.

You're on your branch now. Your commits stay there until you push them.

## 3. Add and commit

Save your scene and assets in Unity, then switch back to GitHub Desktop and open the `Changes` tab. Checking a file adds it to the commit. Unchecked files stay out.

1. Check the box next to each file you want in this commit. The checkbox in the list header selects everything showing.
2. Write one line in `Summary` about what you changed. Use `Description` if that line needs a bit more detail.
3. Click the commit button. It includes your branch name, for example `Commit to feat/planet-landing`.

Summaries on this repo look like this:

```
Added the officer dialogue UI
Fixed the checkpoint respawn height
```

Commit when a piece of work is in a good state. Smaller commits are easier for the rest of us to read later.

When you create a new asset, Unity puts a `.meta` file beside it. Check both boxes. We need that file, or references break for everyone else. If `Library`, `Temp`, or `Logs` show up, leave them unchecked. Unity generates those folders on your machine.

`Discard changes` is in the right-click menu on a file. It deletes edits you haven't committed, so only use it when you mean to throw that work away.

## 4. Push

1. The first time, click `Publish branch`. That creates the branch on GitHub.
2. After that, click `Push origin` whenever you have new commits to share.

Then open a pull request into `dev`.

1. Click `Create pull request`, or use `Branch → Create pull request`. GitHub opens in the browser.
2. Set the base to `dev` and the compare branch to yours.
3. Add a short title and a sentence on what changed.
4. Click `Create pull request`.

If review comes back with notes, commit on the same branch and push again. The pull request picks up those commits.

## 5. Update

Pull the latest `dev` before you start a branch, and again if other people have merged while you're still working. That way you're less likely to edit the same lines as someone else.

### Bring `dev` up to date

1. Commit or discard anything still open, so half-finished work isn't sitting there during the update.
2. Click `Current branch` and select `dev`.
3. Click `Fetch origin`.
4. If the button changes to `Pull origin`, click it.

Your local `dev` should match GitHub after that.

### Bring those changes into your branch

1. Follow the steps above so `dev` is current.
2. Click `Current branch` and switch back to your branch.
3. Choose `Branch → Merge into current branch`.
4. Select `dev` and confirm.
5. If GitHub Desktop creates a merge commit, click `Push origin`.

Switch back to Unity and let it finish importing before you keep working.

If you and someone else changed the same lines, GitHub Desktop lists those files. Open each one, keep the changes that should stay, then commit and push. If you're not sure which version to keep, ask in the team chat before you commit. Please don't force push. It rewrites the branch, and we can lose commits that way.

## Same steps in a terminal

Run these from the project folder.

Clone, once:

```
git clone https://github.com/mbajaman/Waystation.git
cd Waystation
git switch dev
```

Create a branch:

```
git switch dev
git pull
git switch -c feat/planet-landing
```

Add and commit:

```
git add path/to/file
git commit -m "Added the officer dialogue UI"
```

Push the branch the first time:

```
git push -u origin feat/planet-landing
```

Later pushes on that same branch are `git push`.

Update your branch from `dev`:

```
git switch dev
git pull
git switch feat/planet-landing
git merge dev
git push
```

## What the buttons do

| Button | What it does |
|--------|----------------|
| `Fetch origin` | Looks on GitHub for commits you don't have yet. Your files stay as they are until you pull. |
| `Pull origin` | Shows up when your branch is behind. Click it to bring those commits onto the branch you have open. |
| `Publish branch` | Use this the first time you upload a branch. |
| `Push origin` | Sends newer commits up once the branch is already on GitHub. |
| `Current branch` | This is where you switch branches. |
| `Commit to feat/planet-landing` | Saves the checked files on your current branch. The name on the button changes to match. |

Let me know if you have any questions.
