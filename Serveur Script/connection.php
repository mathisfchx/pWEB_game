<?php
$server = "127.0.0.1";
$username = "root";
$password = "z3hZU2UR3LUS"; 
$db = "users";

$connect = new mysqli($server,$username,$password,$db);
if ($connect->connect_error) {
	die("Connection failed: " . $connect->connect_error);
}
?>
