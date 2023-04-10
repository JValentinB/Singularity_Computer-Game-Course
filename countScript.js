function downloadCounter(){
    const anchor = document.querySelector("#DLB");
    anchor.addEventListener("click",function(){
        fetch("./downloaded.txt")
        .then(response => response.text()) 
        .then(textString => {
            console.log(textString);
        });
    })
}
downloadCounter();