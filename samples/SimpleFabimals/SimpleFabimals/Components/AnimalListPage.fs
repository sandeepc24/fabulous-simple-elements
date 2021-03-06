// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace SimpleFabimals.Components

open Fabulous
open Fabulous.XamarinForms
open SimpleFabimals.Models
open SimpleFabimals.Components
open Xamarin.Forms

module AnimalList =
    type Msg =
        | SearchHandlerMsg of SearchHandlers.Msg
        | SelectAnimal of Animal
        
    type CmdMsg = NoOp
        
    type ExternalMsg =
        | NavigateToDetails of Animal

    type Model =
        { PageTitle: string
          IsTopBarDisplayed: bool
          AllAnimals: Animal list
          FilteredAnimals: Animal list }

    let init title isTopBarDisplayed data =
        { PageTitle = title; IsTopBarDisplayed = isTopBarDisplayed; AllAnimals = data; FilteredAnimals = [] }

    let update msg model : Model * CmdMsg list * ExternalMsg list =
        match msg with
        | SearchHandlerMsg (SearchHandlers.Msg.QueryChanged newValue) ->
            let filteredAnimals = model.AllAnimals |> List.filter (fun a -> a.Name.ToLower().Contains(newValue.ToLower()))
            { model with FilteredAnimals = filteredAnimals }, [], []
        | SearchHandlerMsg (SearchHandlers.Msg.AnimalSelected animal) ->
            model, [], [(NavigateToDetails animal)]
        | SelectAnimal animal ->
            model, [], [(NavigateToDetails animal)]

    let navigateToAfterSelectionChanged dispatch (args: SelectionChangedEventArgs) =
        match args.CurrentSelection |> Seq.tryHead with
        | None -> ()
        | Some item ->
            let data = item :?> ViewElementHolder
            let animal = data.ViewElement.GetAttributeKeyed(ViewAttributes.TagAttribKey) :?> Animal
            dispatch (SelectAnimal animal)

    let view model dispatch =
        dependsOn (model.PageTitle, model.IsTopBarDisplayed, model.AllAnimals, model.FilteredAnimals) (fun model (pageTitle, isTopBarDisplayed, allAnimals, filteredAnimals) ->

            // Currently on iOS, when using a CollectionView and Shell (with a top tabbar), the CollectionView goes under the tabbar
            // This is a bug in Xamarin.Forms (https://github.com/xamarin/Xamarin.Forms/pull/6457)
            let contentMargin =
                match isTopBarDisplayed, Xamarin.Forms.Device.RuntimePlatform with
                | true, Device.iOS -> Thickness(0., 40., 0., 0.)
                | _ -> Thickness(0.)

            ContentPage.contentPage [
                ContentPage.Title pageTitle
                ContentPage.ShellSearchHandler (SearchHandlers.animalSearchHandler filteredAnimals (SearchHandlerMsg >> dispatch))
                ContentPage.Content <|
                    CollectionView.collectionView [
                        CollectionView.MarginThickness contentMargin
                        CollectionView.SelectionMode SelectionMode.Single
                        CollectionView.SelectionChanged (navigateToAfterSelectionChanged dispatch)
                        CollectionView.Items (allAnimals |> List.map Templates.animalTemplate)
                    ]
            ]
        )