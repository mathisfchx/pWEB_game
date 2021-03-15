<?php
include('connection.php');

$sql = "Update Inventory set health = (?) , defense = (?),speed = (?) Where Username = (?)";
$st = $connect->prepare($sql);
$st->bind_param("iiis",$health,$defense,$speed,$username);
$health = $_POST["SaveHealth"];
$defense = $_POST["SaveDefense"];
$speed = $_POST["SaveSpeed"];
$username = sha1($_POST['Username']);
$st-> execute();
$st->fetch();
$st->close();



?>