<?php

if (!is_file("./downloaded.txt")) {
    file_put_contents("./downloaded.txt", 0);
}

$dl = file_get_contents("./downloaded.txt");
file_put_contents("./downloaded.txt",++$dl);

?>