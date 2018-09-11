[<RequireQualifiedAccess>]
module ToolbarItem 

open Fabulous.DynamicViews
open Xamarin.Forms
open Xamarin.Forms.StyleSheets

type IToolbarItemProp = 
    abstract name : string 
    abstract value : obj 
    
let internal createProp name value = 
    { new IToolbarItemProp with 
        member x.name = name 
        member x.value = value }    

let Text (value: string) = createProp "text" value 
let OnClick (handler: unit -> unit) = createProp "command" handler 
let Priority (value: int) = createProp "priority" value  
let Order (toolbarItemOrder: ToolbarItemOrder) = createProp "order" toolbarItemOrder
let Icon (icon: string) = createProp "icon" icon 
let StyleId (id: string) = createProp "styleId" id
let ClassId (id: string) = createProp "classId" id 
let AutomationId (id: string) = createProp "automationId" id
let toolbarItem (props: IToolbarItemProp list) = 
    let attributes = 
        props 
        |> List.map (fun prop -> prop.name, prop.value)  
        |> Map.ofList 
    
    let find name = Util.tryFind name attributes
    View.ToolbarItem(?text=find "text", ?command = find "command",?priority = find "priority", ?order = find "order", ?icon = find "icon", ?classId = find "classId",?styleId = find "styleId", ?automationId = find "automationId")