<?php
include('connection.php');

if (isset($_POST["addUsername"]) && !empty($_POST["addUsername"])&& isset($_POST["addPassword"]) && !empty($_POST["addPassword"]) )
{

	$sql = "INSERT INTO authentification (username,password) VALUES (?,?)";
	$st = $connect->prepare($sql);
	$st->bind_param("ss",$username,$password);
	$username = $_POST['addUsername'];
	$password = hash('sha512',$_POST['addUsername'].$_POST['addPassword'].getenv("SERVER_KEY"));
	$st->execute();
	if($st->affected_rows == -1) {
		echo "Error : This Username is already used";
	}else{
		$st->fetch();
		$st->close();
		$sql = "INSERT INTO characters (username,level,experience) VALUES (?,?,?)";
		$st = $connect->prepare($sql);
		$st->bind_param("sii",$username,$level,$experience);
		$username = $_POST['addUsername'];
		$level = ($_POST['addLevel']);
		$experience = ($_POST['addExperience']);
		$st-> execute();
		$st->fetch();
		$st->close();

	}

}
else
{
	echo "Error Serveur";
}
?>
