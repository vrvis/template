﻿open System
open Aardvark.Base
open Aardvark.Base.Rendering
open Aardvark.Base.Incremental
open Aardvark.SceneGraph
open Aardvark.Application
#if __WPF__
open Aardvark.Application.WPF
#endif
#if __WinForms__
open Aardvark.Application.WinForms
#endif


[<EntryPoint;STAThread>]
let main argv = 
    Ag.initialize()
    Aardvark.Init()

    use app = new OpenGlApplication()
    let win = app.CreateSimpleRenderWindow()
	#if __WinForms__
    win.Text <- "Aardvark rocks \\o/"
    #endif

    let quadGeometry =
        IndexedGeometry(
            Mode = IndexedGeometryMode.TriangleList,
            IndexArray = ([|0;1;2; 0;2;3|] :> Array),
            IndexedAttributes =
                SymDict.ofList [
                    DefaultSemantic.Positions, [| V3f.OOO; V3f.IOO; V3f.IIO; V3f.OIO |] :> Array
                    DefaultSemantic.Colors, [| C4b.Red; C4b.Green; C4b.Blue; C4b.Yellow |] :> Array
                ]
        )
       

    let initialView = CameraView.lookAt (V3d(6,6,6)) V3d.Zero V3d.OOI
    let view = initialView |> DefaultCameraController.control win.Mouse win.Keyboard win.Time
    let proj = win.Sizes |> Mod.map (fun s -> Frustum.perspective 60.0 0.1 100.0 (float s.X / float s.Y))


    let sg =
        quadGeometry 
            |> Sg.ofIndexedGeometry
            |> Sg.effect [
                DefaultSurfaces.trafo |> toEffect
                DefaultSurfaces.vertexColor |> toEffect
               ]
            |> Sg.viewTrafo (view |> Mod.map CameraView.viewTrafo)
            |> Sg.projTrafo (proj |> Mod.map Frustum.projTrafo)

    
    let task =
        app.Runtime.CompileRender(win.FramebufferSignature, sg)

    win.RenderTask <- task
    win.Run()
    0
