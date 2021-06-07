<?php

include('connection.php');

$sql = "Update authentification set connected_state = 0";
$st = $connect->prepare($sql);
$st-> execute();
$st->fetch();
$st->close();

?>
