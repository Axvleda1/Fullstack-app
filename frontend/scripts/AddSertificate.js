let certificate;

const user = JSON.parse(localStorage.getItem("user"));
document
  .getElementById("certificateForm")
  .addEventListener("submit", function () {
    console.log("woww");
    const certificateName = document.getElementById("certificateName").value;
    const expiryDate = document.getElementById("expiryDate").value;
    const creationDate = document.getElementById("creationDate").value;
    certificate = {
      CertificateName: certificateName,
      CreationDate: creationDate,
      ExpiryDate: expiryDate,
    };
    console.log(certificate);
    if (user) {
      addCertificate(user, certificate);
    } else {
      console.error("User is not defined or not logged in.");
    }
  });

function addCertificate(user, certificate) {
  fetch("https://localhost:44358/api/Certificates/add", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      User: user,
      Certificate: certificate,
    }),
  })
    .then((response) => {
      localStorage.setItem("certificate", JSON.stringify(certificate));
      if (!response.ok) {
        throw new Error("Network response was not ok");
      }
      return response.text();
    })
    .catch((error) => {
      console.error("Error:", error);
    });
}
