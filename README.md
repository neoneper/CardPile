# CardPile
 A Simple Card Pile system to Unity UI.
This algorithm implementation is for creating linear segments that can be curved along the line.

The entire logic of your deck is up to you. This component is capable of providing the proper position for each card you are going to use at a deck system any, but that's just it!
Use this to give your graph implementation the proper positions, rotations, and constraints.

How to use:
1 - Create your own implementation of the**PileBehaviour**. This can be very simple to start. 
```cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardPile;

public class PileController : PileBehaviour
{
    private void Update()
    {
        UpdatePile();
    }

    protected override void OnNodeAdded(int index)
    {
        Debug.Log(GetNodeObject(index).name);
        Debug.Log(GetNodePosition(index));
        Debug.Log(GetNodeRotation(index));
        Debug.Log(GetNodePositionOffset(index));
    }

    protected override void OnNodeRemoved(int index)
    {

    }

    protected override void OnNodeRemoving(int index)
    {

    }

}

```

2 - Add you component at any GameObject, but remember that it work with UI, so this component need a (RectTransform) as reference to create the nodes as children.
you don't need to add the component in a UI, this is just a convenience. You can place it wherever you want. In my example I put it on the same panel that will be the Parent of all the nodes in the stack

![Configure](https://i.gyazo.com/fe962ace0cb74274119c4a4e3c142dde.png)

3 - Make sure tjat you have a prefab referenced in the field (**Visual Node Prefab**).
This can be anything in the format (RectTransform). It will be used as a VISUAL to show you in real time all the nodes created for the stack.
Take a look at my example: This is a simple image 50x50
![PrefabRef](https://i.gyazo.com/09f0c6a03d5cac9952afa56d37917f75.png)

4 - Play and USE. 


https://i.gyazo.com/2c8169d9f4275c6f70ac448e09d6e18d.mp4
