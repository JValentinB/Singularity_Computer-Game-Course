function downloadCounter(){
    var anchor = document.querySelector("#DLB");
    anchor.addEventListener("click",function(){
        fetch("./downloadResponse.php");
    })
}
downloadCounter();