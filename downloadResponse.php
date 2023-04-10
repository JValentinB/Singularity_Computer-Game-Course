<?php

$file = fopen("downloaded.txt", "w");
$dl = file_get_contents($file);
fwrite($file, $dl+1);
$fclose($file);

/* if (!is_file("./downloaded.txt")) {
    file_put_contents("./downloaded.txt", 0);
}

$dl = file_get_contents("./downloaded.txt");
file_put_contents("./downloaded.txt",$dl+1); */

?>