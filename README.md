# DuplicateHiderNG

[![GitHub release (latest by date)](https://img.shields.io/github/v/release/gOOvER/DuplicateHiderNG?style=flat-square)](https://github.com/gOOvER/DuplicateHiderNG/releases)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=flat-square)](LICENSE)
[![Ko-fi](https://img.shields.io/badge/Ko--fi-Support-F16061?style=flat-square&logo=ko-fi&logoColor=white)](https://ko-fi.com/goover)

> **DuplicateHiderNG** is an extension for the [Playnite](https://playnite.link/ "Playnite - video game library manager") video game library manager that automatically hides duplicate copies of games across multiple digital distribution platforms (Steam, GOG, Epic, Amazon, RSI, etc.) based on configurable source priorities.
>
> Originally created as [DuplicateHider](https://github.com/felixkmh/DuplicateHider) by felixkmh, now rebranded and actively maintained as **DuplicateHiderNG** by [gOOvER](https://github.com/gOOvER).

[Playnite Forum Thread](https://playnite.link/forum/thread-308.html) | [GitHub Repository](https://github.com/gOOvER/DuplicateHiderNG)

## Extension Settings

![Extension Settings](https://user-images.githubusercontent.com/24227002/132069628-437aedbd-17c0-4277-8cca-1c36af65cf59.png "Plugin Settings")


### Priorites

Reordable list of Sources/Libraries that assigns each Source a priority according to their position in the priorites list. Higher position meaning a higher priority (lower value).
DuplicateHiderNG will use this priority to compute a score for each copy of a game,

```csharp
    Score(game) = Priority(game.Source) - priorityList.Count * IsInstalled(game)?1:0
```

and copies are sorted in ascending order by their score. All but the first game are then hidden.

### Update Rank Automatically

If enabled, keeps the scores updated (and hides games accordingly) when changes to the library are made and hides games automatically when Playnite launches or settings are changed.

### Game Filters

The list of games that is checked for duplicates can be filtered by the _Include Platforms_, _Exclude Sources_ and _Exclude Categories_ filters.

Enable _Include All Platforms_ to bypass the platform filter entirely — all platforms, including newly added ones, will be considered.

### Ignored Games

Additionally to the aforementioned filters, games in the _Ignored Games_ list are also not considered by DuplicateHiderNG.
To remove entries from the list, select one or more entries and right click to remove them.  
Also, enabling the _Add manually hidden/revealed Games_ option will cause games which hidden states are changed outside of DuplicateHiderNG to be added to that list.

Enable _Never Hide Installed_ to ensure that installed copies are never hidden, regardless of source priority.

### Display String & Show Other Copies

![Other Copies](https://i.ibb.co/r7FGKfw/grafik.png "Other Copies")

If the _Show Copies in Game Menu_ option is enabled, right clicking a game will show other copies of the clicked game in the context menu.
The _Display string for other copies_ is used to generate the string that is used for a game's entry in the context menu.
Variables in the format `{Prefix'Variable'Suffix}` are only evaluated to a non-empty string, if `Variable` can be evaluated.
If `Variable` can be evaluated, the prefix and suffix strings are also shown as is.
If no prefix or suffix is needed, `{Prefix'Variable}`, `{Variable}` and `{Variable'Suffix}` can also be used.
To bring up a list of available variables, right click inside the text box and click on a variable to insert it.

The example above used `{Name} [{Installed}{ on 'Source}{, ROM: 'ImageNameNoExt}]` as the display string.

The `{Library}` variable resolves to the human-readable name of the importing library plugin (e.g. `Steam`, `GOG`). Use this instead of `{Source}` when you want the plugin name rather than the manually set source label.

### UI Integration

![Icon Stack](https://i.ibb.co/NjxdSZC/grafik.png "Icon Stack")

This extension provides a custom UI element that can be integrated by theme creators.
To use it, the Theme you are using needs to support it and the _UI Integration_ option must be enabled. Available starting with Playnite 9.

The custom UI element consists of a stack of icons associated with the copys of a game. Clicking on an icon will select this version of the game, double clicking launches it. Slightly grayed out icons indicate that a copy is not installed. An expample is shown above.

- _UI Integration_: Enable UI integration for Themes that support it.
- _Enable Theme Icons_: Themes can provide their own icons for the different game libraries, like Steam, GOG and so on. If this option is enabled, those will be used if available.
- _Prefer User Icons_: Users can also supply their own icons by placing them into a special folder. This folder can be opened by pressing the _Open user icon folder_ button. If this option is enabled, existing user icons will always be preferred, only falling back to the ones included with the Theme or the default ones, if no user icon can be found for a given source.
- _Show icon if there is only one copy_: By default, icons are only displayed when the associated game has at least one additional copy. Enabling this option will always show an icon.

#### User Specified Icons

The extension comes with a set of predefined icons for common libraries (from https://icon-icons.com/pack/Material-Design/2248, under the [Apache 2.0](https://www.apache.org/licenses/LICENSE-2.0) License), but users can also specify their own. For DuplicateHiderNG to find those icons, they need to be named like the sources in the _Priority List_, for example `Steam.ico` or `Ubisoft Connect.png` and then placed into the _source\_icons_ folder that can be found by pressing the _Open user icon folder_ button in the plugin settings.

### Custom Groups

Games can be manually grouped together via _Extensions_ -> _DuplicateHiderNG_ -> _Custom Groups_. Games in the same custom group are always treated as duplicates of each other.

Enable _Sort Custom Groups by Name_ to display custom groups alphabetically in the context menu.

## Extension Menu

![Game Menu](https://i.ibb.co/G9r0BZ4/grafik.png "Game Menu")

Under _Extensions_ -> _DuplicateHiderNG_ functions to manually hide and reveal duplicates can be found. Also, currently selected games can either be added or removed from the _Ignore List_.

## Theme Integration

The custom UI element can be intgrated into a Theme by, for example, placing

```xml
<ContentControl x:Name="DuplicateHider_SourceSelector" DockPanel.Dock="Right" MaxHeight="{Binding ElementName=PART_ImageIcon, Path=Height}"/>
```

into the `DetailsViewItemTemplate.xaml` file where appropriate. See Playnite documentation for more information. At runtime, Playnite will set its content to a Control holding a StackPanel of icons (ContentControls), depending on the GameContext. This might look like this

![Icon Stack](https://i.ibb.co/NjxdSZC/grafik.png "Icon Stack")

when placed in a ListViewItem in the DetailsView. See Playnite Documentation to see where else custom UI elements can be used. Go [here](UiIntegrationExamples/) for  better examples.

### SourceSelector

There are up to 10 SourceSelectors, ```DuplicateHider_SourceSelector```, ```DuplicateHider_SourceSelector1```, ```DuplicateHider_SourceSelector2```, and so on that you can use by using their names as the name of a  ```ContentControl``` in a supported template or view. 

For each SourceSelector, you can provide styles for their ```StackPanel``` and the icons which are just ```ContentControls```s (with the Icon as its content if no style is provided). The styles need to have the keys ```DuplicateHider_IconContentControlStyle``` and ```DuplicateHider_IconStackPanelStyle``` (or with the added number for the other ones). 

SourceSelector utilizes a cache for the UI elements and Icons and is suitable for use in the Item Templates for the GridView and DetailsView. 

> __Note__: The icon ui elements are also recycled when a SourceSelector's IsVisible property is false, so when animating the opacity, the Visibility property should be Hidden or Collapsed when the opacity is 0.

Example Styles can be found [here](UiIntegrationExamples/DuplicateHider_SourceSelector_Styles_Example.xaml). Each Icon ContentControl has a corresponding ListData object set as its DataContext. An overview of available Properties can be found in the next section.

> __Note__: ```DuplicateHider_SourceSelectorN``` is only registered if either ```DuplicateHider_IconContentControlStyleN``` or ```DuplicateHider_IconStackPanelStyleN``` is found. ```DuplicateHider_SourceSelector``` is always registered.

### ContentControl

There are also 10 instances of ```DuplicateHider_ContentControl``` (in case you want to use several in the same View), numbered like the SourceSelector. These basically only provide a DataContext and need a Style/Template to do anything. To apply a Style, set it as the __Tag__ of a ContentControl with x:Name="DuplicateHider_ContentControl" (or one of the 9 others):

```xml
<Style x:Key="DuplicateHider_ContentControlHeader_Style" TargetType="ContentControl"> ... </Style>
...
<ContentControl x:Name="DuplicateHider_ContentControl" Tag="{DynamicResource DuplicateHider_ContentControlHeader_Style}"/> 
```

The styled ContentControl has access to the following DataContext:

 ```csharp
DataContext = {
    ListData CurrentGame;                  // Game currently in view
    ObversableCollection<ListData> Games;  // Data of copys of current game, including itself
    Boolean MoreThanOneCopy;               // Games.Count() > 1
    Boolean SwitchedGroup;                 // True iff the newly selected Game 
                                           // is not a copy of the previously selected one
}

class ListData {
    Playnite.SDK.Models.Game Game;
    Boolean IsCurrent;        // True if this copy is currently in view.
    BitmapImage Icon;         // Source Icon
    String SourceName;        // Source name. Use this rather than Game.Source.Name, 
                              // because Source might be null.
    String DisplayString;     // Expanded display string as defined in plugin settings.
    ICommand LaunchCommand;
    ICommand SelectCommand;
    ICommand InstallCommand;
    ICommand UninstallCommand;
}
```

The Icons are still cached for this component, but using this in the DetailsViewItemTemplate or GridViewItemTemplate may cause a lot of objects to be created whenever the views are switched or view filters are changed, depending on the used Template.

Example Styles can be found [here](UiIntegrationExamples/DuplicateHider_ContentControl_Style_Examples.xaml). A selfcontained example of multiple DuplicateHider_ContentControls in the DetailsViewGameOverview.xaml can be found [here](UiIntegrationExamples/UiIntegrationDetailsViewExample). See [Example Overview](#example-overview).

### Theme Icons

Themes can also supply their own source icons, by adding entries to the resource dictionary and adding the icon files into the appropriate folder. The entries need to have the key  `DuplicateHider_SOURCENAME_Icon`, where _SOURCENAME_ needs to be replaced by the name of the source as seen in the _Priority List_. For example, if you want to add an icon for Uplay aka Ubisoft Connect, you might add

```xml
<BitmapImage x:Key="DuplicateHider_Ubisoft Connect_Icon" UriSource="{ThemeFile 'Images/Icons/ubisoft.png'}" RenderOptions.BitmapScalingMode="Fant"/>
```

to the `Media.xaml` file and place `ubisoft.png` into the `Image/Icons` folder. A default icon can be specified by giving it the key ```DuplicateHider_Default_Icon```
> __Note__: Theme Icons are disabled by default and need to be enabled in the settings in order for them to be used.

### Icon Number Limit

By adding

```xml
<sys:Int32 x:Key="DuplicateHider_MaxNumberOfIcons">4</sys:Int32>
```

to the resource dictionary, a Theme can also specify the maximum number of icons per element. In the example above, it is set to 4, which is also the default if no entry is supplied. Each ```DuplicateHider_MaxNumberOfIconsN``` will apply to both ```DuplicateHider_SourceSelectorN``` and ```DuplicateHider_ContentControlN```. A value less than 1 means no limit on the number of icons.

### Example Overview

| File | Description | Preview |
|------|-------------|---------|
| [DetailsView<br/>ItemTemplate.xaml](UiIntegrationExamples/UiIntegrationSimpleExample/DetailsViewItemTemplate.xaml) | Places clickable Icons next to the game name in the DetailsView. | ![grafik](https://user-images.githubusercontent.com/24227002/113683300-55130b80-96c4-11eb-9f27-366ef4cb4bad.png) |
| [DetailsView<br/>ItemTemplate.xaml](UiIntegrationExamples/UiIntegrationFadingExample/DetailsViewItemTemplate.xaml) | Similar to above example, but here the icons fade in and out when the mouse is over a ListItem. |  |
| [DetailsView<br/>ItemTemplate.xaml](UiIntegrationExamples/UiIntegrationFadingExample2/DetailsViewItemTemplate.xaml) | Similar to above example, but here the width of the StackPanel is also animated such that long game names are pushed away when it appears. |  |
| [DetailsView<br/>GameOverview.xaml](UiIntegrationExamples/UiIntegrationDetailsViewExample/DetailsViewGameOverview.xaml) | Contains Styles for the DuplicateHider_ContentControl and uses them to display available sources as header and adds an conditional extension to the PlayButton. | ![grafik](https://user-images.githubusercontent.com/24227002/113638466-363a5800-9677-11eb-869d-e73507df7928.png) |
| [DuplicateHider_<br/>ContentControl_<br/>Style_Examples.xaml](UiIntegrationExamples/DuplicateHider_ContentControl_Style_Examples.xaml) | Two Styles for DuplicateHider_ContentControl. ```DH_ContentControl_Style``` contains some animations to indicate the current game.| DH_ContentControl_Simple_Style:<br/> ![grafik](https://user-images.githubusercontent.com/24227002/113685683-c94eae80-96c6-11eb-9241-3675010d25e6.png) DH_ContentControl_Style:<br/> ![grafik](https://user-images.githubusercontent.com/24227002/113685711-d075bc80-96c6-11eb-92d1-c9c2abba3900.png) |
| [DuplicateHider_<br/>SourceSelector_<br/>Styles_Example.xaml](UiIntegrationExamples/DuplicateHider_SourceSelector_Styles_Example.xaml) | Contains fairly barebones Styles for the DuplicateHider_SourceSelector. Basically the default Styles if none are provided for a SourceSelector. ```DuplicateHider_IconContentControlStyle``` for the Icons and ```DuplicateHider_IconStackPanelStyle``` for the StackPanel |  |
| [ListGameItem<br/>Template.xaml](UiIntegrationExamples/UiIntegrationFullscreenExample/ListGameItemTemplate.xaml) | Simple example for the ```SourceSelector``` on a game card in a fullscreen theme. | ![grafik](https://user-images.githubusercontent.com/24227002/113946975-dc1dcc00-9809-11eb-8276-b9bc1290344e.png) |
| [GameDetails.xaml](UiIntegrationExamples/UiIntegrationFullscreenExample/GameDetails.xaml) | Example for a DetailsView in a fullscreen theme using ```DuplicateHider_ContentControl```. | ![grafik](https://user-images.githubusercontent.com/24227002/113946801-8ba66e80-9809-11eb-8a2e-b9b537d5a2be.png) |
| [DetailsView<br/>ItemTemplate.xaml](UiIntegrationExamples/UiIntegrationAnimatedSourceSelectorExample/DetailsViewItemTemplate.xaml) | Example for SourceSelector in DetailsViewItemTemplate with highlighting and animations revealing the source name on hover. | ![grafik](https://user-images.githubusercontent.com/24227002/114274054-fb407780-9a1c-11eb-97ff-e505f4b9af66.png) |

### Showcase Themes

- **[Penumbra Themes Suite](https://github.com/gOOvER/Penumbra-Themes)**: Full, premium native integration for Penumbra Dawn (Desktop), Penumbra Night (Desktop), and Penumbra Blur (Fullscreen) featuring animated source selectors, custom badges, and frosted glass pill containers.
- **Classic Themes**: Legacy showcase themes ([Night](https://github.com/felixkmh/DH_Themes/tree/main/source/Night)).

Preview:
|View|Preview|
|----|-------|
|Desktop - DetailsView (Night)| ![grafik](https://user-images.githubusercontent.com/24227002/115793144-ea541680-a3cb-11eb-9138-957c8b33bd81.png) |

## Support & Donate

If you enjoy DuplicateHiderNG and want to support its ongoing development, feel free to support on Ko-fi:

[![Ko-fi](https://img.shields.io/badge/Ko--fi-Support%20gOOvER-F16061?style=for-the-badge&logo=ko-fi&logoColor=white)](https://ko-fi.com/goover)

Direct link: [https://ko-fi.com/goover](https://ko-fi.com/goover)

## License

This project is licensed under the **MIT License**. See [LICENSE](LICENSE) for details.


