# Elarion

Elarion is a collection of tools for [Unity](http://www.unity3d.com/). It includes quite a few things, but here are the highlights:

 * Scriptable Object tooling enabling you to handle the data-binding in a convenient and very "Unity" way. Or just build you whole architecture using ScriptableObjects, you call.
 * Custom event system that helps leverage said scriptable objects.
 * Minimal UI system which can easily scale to handle very complex use-cases. The goal with this is to reuse as much of Unity's GUI system as possible and add very little to make it convenient to work with.
 * A set of graphic components styled according to the Material Design guidelines that I prepared to be used for easy UI building (A tutorial is in the works).
 * Generic custom inspector in which you can add conditional rendering, buttons, and a lot of other useful tidbits. And all that works with *your* property drawers.
 * Lots of property drawers to help display data in the editor.
 * Variety of different tools with various functionality. Those vary from extension methods to an IconManager that allows for assets to inherit icons.
 
 This thing is huge, I know, and I'm working on breaking it down into a couple of smaller repositories. This one will always be the central hub, but expect to see some submodules in the near future. 

## Getting Started

Adding Elarion to your project can be as easy as downloading it and extracting it anywhere in your **Assets Folder**. Alternatively, for easier updates (and extra points), you can add it as a *git submodule*.

### Adding Elarion as a git submodule

If you're not familiar with git submodules, you can get started [here](https://git-scm.com/book/en/v2/Git-Tools-Submodules).

#### Adding the submodule 

I'll describe a simple setup in which Elarion lives in the root directory of the project - modify it to suit your needs. 

First, go to your project's directory and run:

```
cd Assets
git submodule add https://github.com/jedybg/Elarion.git
```

This will clone the project in a the Elarion directory. You'll need to commit the folder and the .gitmodules file (tracking all submodules) to your repository.

#### Updating the submodule

The great thing about submodules is that they're simple repositories. You can easily go to your project's directory and use *git pull* to update to the latest version.

```
cd Assets/Elarion
git pull # or any other git command
```

Just remember that this will generate a changeset in *your repository* which you'll have to later commit. 


## Contributing

The project is, of course, open to contributions. Just fork it and post a pull request.

## Authors

* **Jordan Georgiev** - *The one to (git) blame* - [jedybg](https://github.com/jedybg)

## License

This project is licensed under the MIT - see the [LICENSE](LICENSE) file for details.
