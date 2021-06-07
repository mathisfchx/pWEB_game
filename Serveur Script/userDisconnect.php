<?php
include('connection.php');

if (isset($_POST["Username"]) && !empty($_POST["Username"]))
{
	$sql = "UPDATE authentification set connected_state = (?) Where Username = (?)";
	$st = $connect->prepare($sql);
	$st->bind_param("is",$connected_state,$username);
	$connected_state= 0 ;
	$username = $_POST['Username'];
	$st-> execute();
	$st->fetch();
	$st->close();
	echo "Connected_state mis à jour" ; 

}
?>
