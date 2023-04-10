function downloadCounter(){
    const anchor = document.querySelector("#DLB");
    const fs = require('fs');
    anchor.addEventListener("click",function(){
        fetch("downloaded.txt")
        .then(response => response.text()) 
        .then(textString => parseInt(textString, 10))
        .then(num => fs.writeFile("downloaded.txt",num+1))
        });
}
downloadCounter();