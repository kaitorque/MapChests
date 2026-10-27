# Test Cases

Below are some tests cases that should be run to confirm updates work as expected

## Chests

| Case                 | Expected Outcome                                |
| -------------------- | ----------------------------------------------- |
| Artifact of Delusion | Chests should become visible and be highlighted |
| Artifact of Delusion | Chests should still be interactable             |
| Used chest           | Should disappear and highlight should disable   |
| Used chest           | Should not have a collision                     |

## Shops

| Case              | Expected Outcome                                                |
| ----------------- | --------------------------------------------------------------- |
| Executive Card    | Shops can be used multiple times                                |
| Shipping manifest | Free shops should be highlighted                                |
| Shipping manifest | Used free shops should properly hide/fade (Including particles) |
| Shipping manifest | Used free shops have no collision                               |
| Used Shop         | Should disappear and highlight should disable                   |
| Stage change      | Should not remove item highlight in shops                       |
| Setting change    | Should not remove item highlight in shops                       |

## Settings

| Case              | Expected Outcome                           |
| ----------------- | ------------------------------------------ |
| Changing settings | Should take effect immediately             |
| Changing settings | Should not make used stuff reappear        |
| Fade              | Stuff will fade as expected                |
| Hide              | Stuff will hide                            |
| Different colors  | Categories can have different colors       |
| Used              | Highlights are removed if remove from used |

## General

These cover general cases, like restarting a run, going to the next stage

| Case               | Expected Outcome                                             |
| ------------------ | ------------------------------------------------------------ |
| Stage change       | All configured interactables are highlighted                 |
| Stage change       | No errors about null pointers in console                     |
| Restarting a run   | No errors about null pointers, interactables are highlighted |
| Starting a new run | Interactables are highlighted                                |
