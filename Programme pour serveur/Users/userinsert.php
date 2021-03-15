<?php
include('connection.php');

if (isset($_POST["addUsername"]) && !empty($_POST["addUsername"])&& isset($_POST["addPassword"]) && !empty($_POST["addPassword"]))
{


	$sql = "INSERT INTO Authentification (Username,Password) VALUES (?,?)";
	$st = $connect->prepare($sql);
	$st->bind_param("ss",$username,$password);
	$username = sha1($_POST['addUsername']);
	$password = sha1($_POST['addPassword']);
	//try {
		$st->execute();
		if($st->affected_rows == -1) {
			echo "Error : This Username is already used";
		}else{ 
			$st->fetch();
			$st->close();
			$sql = "INSERT INTO Inventory (Username)  VALUES (?)";
			$st = $connect->prepare($sql);
			$st->bind_param("s",$username);
			$username = sha1($_POST['addUsername']);
			$st-> execute();
			$st->fetch();
			$st->close();
			
		}
	//}catch (Exception $e){
		//echo "Error : This Username Already exists ".$e;
	//}finally{
	//	$st->fetch();
	//	$st->close();
	//}
}
else 
{
	echo "Error Serveur";
}
?>


