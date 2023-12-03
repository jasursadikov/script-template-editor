> [!IMPORTANT]
> Currently supports only Windows.

# Script Template Editor

Allows you to customize Unity's default script templates.

To customize your script templates, go to **"Edit/Preferences/Script Templates"**

![Preferences](https://github.com/jasursadikov/script-template-editor/blob/master/Images~/img.png)

## How to install?

1. Open **"Windows/Package Manager"**
2. Click '+' button and press **"Add package from git URI..."**
3. Paste https://github.com/jasursadikov/script-template-editor.git#master
4. Press **Enter** and Enjoy!

## How to use?

1. Open **"Edit/Preferences"**
2. Find **"Script Templates"**
3. Choose script template that you want to edit
4. Press **"Save"**
5. To restore to default values, press **"Reset to Default"**

## How does it work?

In Unity you have your default templates in Unity editor folder. To override them, this package does use powershell script to replace the content of your previous script template.

Templates are changed only for one installation, you should apply your edits for each Unity version you have on your machine.
