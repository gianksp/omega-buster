----------------------------------------------
            Xffect Editor Free Edition
 Copyright 2012- Shallway Studio
                Version 5.2.0
            shallwaycn@gmail.com
----------------------------------------------

Support:
http://forum.unity3d.com/threads/xffect-editor-pro-powerful-tool-to-create-amazing-effects.142245/

--------------------
 How To Update Xffect
--------------------
1. In Unity, File -> New Scene
2. Delete the Xffect folder from the Project View.
3. Import Xffect from the updated Unity Package.


--------------------
Release Notes
--------------------
Version 5.2.0
-NEW: Added "Preview Tool" to capture each xffect's thumbnail.
-NEW: Added "Xffect Browser" to manage all your xffects in current project. click Window/Xffect/Xffect Browser to check it.
-NEW: EffectLayer with subemitter enabled can also be updated in editor now.

Version 5.1.1
-FIX: Fixed a shader compiled error in Unity5.
-NEW: Added Gizmos to show Emitter shapes and Directions.


Version 5.1.0
-FIX: Now the CustomMesh's rotation can inherit transform rotation correctly.
-NEW: Added a new Effect pack, please take a look at the "EffectLibrary\X-Projectile" folder to learn more.

Version 5.0.3
-FIX: Ready for Unity5
-FIX: Spline obj editor performance improved.
-NEW: Added a new tool: XClipArea. You can use it to clip any object in world space(beta version)

Version 5.0.2
-NEW: Added a high quality tornado effect: EffectLibrary\Prefabs\BigEffects\tornado.prefab

Version 5.0.0
-NEW: Added a spline editor, click "GameObject -> Create Other -> XSpline" to learn more.
-NEW: Added a new Render Type: "Spline Trail".
-NEW: Added a new Emitter type: Spline Emitter.
-NEW: Added a new Direction type: Spline direction.
*Please check out Tutorial\Spline folder to learn the details*

-NEW: Added TimeScale Controller to EffectLayer, you can use it to control the playback time dynamically.
*Please check out "EffectLibrary\Prefabs\Spline\SkullSWirl.prefab" to learn more*

-FIX: RibbonTrail supports random start scale now.
-FIX: Camera Effects performance improved.
-FIX: Point emit pos inerit client rotation now.

Version 4.5.3
-NEW API: You can retrieve effectlayers dynamicly by using XffectObj.EffectLayerList.
-FIX: RibbonTrail performance improved.
-FIX: EmitByCurve method works properly now.
-FIX: Xffect Gizmos and mesh wireframes will not be displayed while updating it in editor.
-FIX: X-WeaponTrail is compatible with scaleTime now.
-FIX: Rope Renderer's start scale works properly now.
-FIX: Modifiers' magnitude can be influenced by Curve01 now.

Version 4.5.2
-FIX: Fixed a bug that non-looping uv animation never play the first frame..


Version 4.5.1
-FIX: Now you can make particles' movement simulate in world space by checking off "inherit client rotation".
-FIX: Fixed a bug that "random uv start" option never reaches the last frame.

Version 4.5.0
-NEW: Added X-WeaponTrail plugin to this package, It's the best weapon trail solution in unity so far, Please check out this folder: Xffect\EnhancementTools\XWeaponTrail.
-NEW: Added a Flow shader, It's very suitable for making some flow field like the health ball in Diablo3, Please check out this folder: Xffect\Tutorial\AdvanceShader\Flow.

Version 4.4.4
-FIX: Optimized the performance of API "SetGravityGoal()".
-FIX: Fixed the "Spherical Billboard" example.

Version 4.4.3
-FIX: Fixed a bug that the field of "Playback Time" in control window is not visible in unity 4.5.

Version 4.4.2
-FIX: Fixed the Z-sorting problem with the glow per obj feature.
-FIX: Fixed a nullreference error, added a check method in XffectComponent.Start.CheckEditModeInited function.
-FIX: FIX a bug that if there has no cameras in the scene, the camera shake component will not be inited correctly.

Version 4.4.1
-FIX: re-enabled updating in editor for 'camera shake' and 'camera effect' event.
-FIX: adjusted the width of the color gradient field in inspector, now all the inspector label width are uniform.

Version 4.4.0
-FIX: The sprites in the scene view will face the editor camera now.
-FIX: You can set another camera to XffectComponent now: myXffect.MyCamera = anotherCamera.
-NEW: Obsoleted the color gradient editor and Added a new one, it's the same as shuriken's color gradient editor.

Version 4.3.2
-FIX: Camera rendering upside down problem completely solved, thanks for gtotoy's solution.
-NEW: Multi EffectLayer editing supported(still in beta, be careful to use it. If you messed up your effectlayers, use ctrl+z to revert them).

Version 4.3.1
-FIX: re-enabled the auto activate feature in the Start() method of xffect obj.

Version 4.3.0
*-FIX: EffectLayer editor performance has been improved, is's super fast now and no jerky when update it in editor.
-FIX: Fixed a very old problem that if you have any xffect obj in the scene, there is always a "*" in the unity title bar which means the scene needs to re-save. Many thanks to Arcanor's feedback, now this issue has been solved and the xffect editor is more stable.
-FIX: Effect Visibility check method fixed, now uncheck the "update when offscreen" option should work properly.
-NEW: Added a new effect: "energy_shield.prefab" in "EffectLibrary/Other" directory.
-NEW: Added a new option "start offset" in UV Scroll config.
-NEW: Added a new UV change type: "uv scale" in UV Config. You can use it to change the uv dimensions dynamically.

Versin 4.2.1
-FIX: The "Cone" and "CustomMesh"'s direction can inherit the client's rotation now.
-FIX: API "SetColor()" should work peroperly now.
-NEW: Added a new effect: "boost wind" to "EffectLibrary/Mobile" directory.
-NEW: Added option : "sync with client" in RibbonTrail, you can use this option to synchronize all the trail's nodes with the client's position.
(The protect_ring.prefab has also been improved with this option.)
-IMPROVED: Improved the performance of "Glow" and "GlowPerObj" camera effect according to this warning:http://forum.unity3d.com/threads/191906-4-2-Any-way-to-turn-off-the-quot-Tiled-GPU-perf-warning-quot/page2


Version 4.2.0
-NEW: Added a new render type: 'Spherical Billboard', you can use it to make realistic explosions and fogs, check 'Tutorial/SphericalBillboard' folder to learn more.
-NEW: Added 'Playback time' option in the 'update in editor' control window.
-FIX: Fixed some camera shake bugs.
-FIX: Fixed a bug that the camera effect events can't be removed correctly since version 4.1.0.


Version 4.1.5
-NEW: Added a button "Put to Scene" in Xffect prefabs' inspector, you can use this button to preview xffect prefab easily.
-FIX: Fixed that while updating in editor at EffectLayer the effect becomes very jerky.
-DEL: Obsolete parameter "emit loop count" and "delay after each loop" in "Emitter Config" since they are useless but bring confusions.

Version 4.1.4
-FIX: Fixed a bug that the xffect will become jerky when the 'Time.timeScale' is very small.
-NEW: Added option 'Max Fps' in XffectComponent, you can use this option to limit xffect updates in a certain fps.

Version 4.1.3
-NEW: Added option "use with 2d sprite" in XffectComponent, you can use it to integrate xffect with 2d sprite in unity 4.3 now.
-FIX: Fixed a bug that the Glow Per Obj is rendering upside down in version above 4.1.0.
-FIX: Fixed a bug that the Direction is not inherited from client correctly. 


Version 4.1.2
-NEW: Added option "uniform random start scale" in Scale Config.
-NEW: Added a control window for EffectLayer to update the parent Xffect in Editor.
-FIX: The default scale curve is zoom out to the extends of the graph now.


Version 4.1.0
-NEW: Compatible with unity 4.3.
-NEW: Added option "update when offscreen" in XffectComponent;
-NEW: Added a control window in the bottom left corner of the scene view.
-NEW: You can control the camera shake event by curve now.
-IMPROVED£º The camera effect code has been rewritten, all the camera components are combined into one component, and you can assign a priority to each camera effect now.
-FIXED: The Camera shake force will be added relative to the camera transform now, which means this event will no longer conflict with other camera component any more.
-FIXED£ºFixed some bugs that 'Glow Per Obj' shader and 'displacement-dissolve' shader are not working properly in some cases.
-FIXED: Fixed a bug that the 'Glow Per Obj' shader will make the skybox glow..
-FIXED: Fixed a bug that the camera will not be assigned correctly if you have multiply camera in the scene.

Version 4.0.0
-Documentations and tutorials are now included in this package, please check it out! 
-This version has many changes, please check UPGRADE_NOTES.TXT before updating!

