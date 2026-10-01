# Unity-2D-Destruction
Unity 2D Destruction is a basic tool for breaking 2D sprites into fragments for awesome destruction effects!!!

[See it in action!](http://gfycat.com/BadQualifiedEyelashpitviper)

[Unity Forum] (http://forum.unity3d.com/threads/free-open-source-unity-2d-destruction.382416/)

[Tutorial Video] (https://youtu.be/pe4_Dimk7v0)

## Instructions for Basic Use
* Import the Unity 2D Destruction package
* drag a sprite into your scene
* Add an Explodable component and a PolygonCollider2D or BoxCollider2D
* Set your parameters and click Generate Fragments (repeat until you are satisfied with your fragments)
* During gameplay, call explode() on the Explodable component to destroy the original sprite and activate the fragments

## Detailed Explanation of Parameters
**Allow Runtime Fragmentation**: Set this to true to generate your fragments during gameplay instead of in the editor. When you call "explode()" the fragments will be generated and the original destroyed.
The fragmentaiton operation isn't the fastest so I don't really recommend this.

**Shatter Type**: Can set this to generate triangular fragments or more "realistic" voronoi fragments.

**Extra Points**: Ordinarilly the fragments are generated using the points of the collider. With this parameter you can add any number of random points inside the bounds of the collider.
Use this to get more random and interesting pieces.

**Subshatter Steps**: For each subshatter step, the fragmentation operation will be run on each generated fragment. For example if it's set to 2 your sprite will be fragmented, then each fragment
will be fragmented, and then those fragments will be fragmented again. I wouldn't set this above 2 and usually 1 is enough.

**Fragment Layer**: The layer you wish the fragments to be set to

**Sorting Layer**: The sorting layer you wish the fragments to be set to

**Order In Layer**: The order in layer you wish the fragments to be set to

## Crust port

This fork is being ported to C with [crust](https://github.com/brentharts/crust):
the runtime scripts only (the Editor code is not part of it). The MonoBehaviours
go through crust's unity_pack; the fracture geometry (`Unity-delaunay`,
`clipper_library`) through crust's C# subset; the fragments' physics through
[Box2D-Packed](https://github.com/crustos/box2d). Every change keeps the source
plain C# that Unity compiles as before.

### Conventions

* **`[MaxInstances(N)]`** (`Scripts/MaxInstancesAttribute.cs`) marks a class of
  which at most N exist at once. Unity ignores it; crust allocates such a class
  from an arena of N slots -- a reference is a plain pointer, as in C#, and the
  arena is released in bulk -- and sizes unity_pack's tables to N. `Explodable`
  is `[MaxInstances(255)]`; the Delaunay graph classes are arena classes too:
  `Site` 1024, `Vertex` 2048, `Edge` 3072, `Halfedge` 6144, `Triangle` 2048,
  `Node` 1024 (generous for now: a Voronoi diagram of S sites has at most
  3S edges and 2S vertices; to be tuned down once fractures are measured).
* **`#if !CRUST`** keeps Unity-only code out of the C build: crust defines
  `CRUST`. The debug `ToString` overrides (`Edge`, `Halfedge`, `Site`,
  `Vertex`, `Circle`) are behind it.
* **Plain loops for delegates**: `List.FindAll(delegate ..)` in `Voronoi` and
  `DelaunayHelpers` is the loop it stands for, and `Triangle` adds its three
  sites one by one instead of a collection initializer -- same results, in a
  form the C# subset translates.

### Status

9 of the runtime files translate to C (`ICoord`, `LR`, `Edge`, `Circle`,
`LineSegment`, `Polygon`, `Winding`, `IDisposable`, `MaxInstancesAttribute`);
every other one is refused by crust with a line and a reason. The next steps,
most common blocker first:

1. translating the library's files as one unit -- `Triangle`, `Vertex`,
   `Voronoi`, `SiteList` and `EdgeReorderer` use an interface from another
   file (`IDisposable`, `ICoord`);
2. `List.Sort` with a comparison (`Site`, `DelaunayHelpers`);
3. `new Halfedge[n]` with the arena class in another file (`EdgeList`,
   `HalfedgePriorityQueue`) -- also the one-unit step;
4. `List.AddRange` (`SpriteExploder`), and `clipper.cs`'s `PolyNode` tree
   (arena classes, as for Delaunay).

Clone this repository beside crust and its fast tests check the port
(`python3 tools/unity_pack_test_fast.py TestUnity2DDestruction`); crust's
UNITY_PACK.md, "Unity-2D-Destruction", has the same status.
