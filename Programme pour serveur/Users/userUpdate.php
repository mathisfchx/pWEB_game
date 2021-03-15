<?php
include('connection.php');

$username = $_POST['addUsername'];
$ID = $_POST['addID'];
$password = $_POST['addPassword']

$whereField = $_POST['whereField'];
$whereCondition = $_POST['whereCondition'];


$sql = "update users set ID = '".$ID."',Username='".$unsername."',Password='".$password"' where ".$whereField."='".$whereCondition."'";
mysqli_query($connect , $sql);

?>

