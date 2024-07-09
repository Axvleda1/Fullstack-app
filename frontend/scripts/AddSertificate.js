let certificate;

let user;
user = JSON.parse(localStorage.getItem("user"));
console.log(user);
document
  .getElementById("certificateForm")
  .addEventListener("submit", function (event) {
    event.preventDefault();

    const certificateName = document.getElementById("certificateName").value;
    const expiryDate = document.getElementById("expiryDate").value;
    const creationDate = document.getElementById("creationDate").value;

    if (!certificateName || !creationDate || !expiryDate) {
      alert("Please fill out all fields.");
      return;
    }

    certificate = {
      CertificateName: certificateName,
      CreationDate: creationDate,
      ExpiryDate: expiryDate,
    };

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
    body: JSON.stringify({ User: user, Certificate: certificate }),
  })
    .then((response) => {
      localStorage.setItem("certificate", JSON.stringify(certificate));
      if (!response.ok) {
        throw new Error("Network response was not ok");
      }
      return response.text();
    })
    .then(() => {
      location.reload();
    })
    .catch((error) => {
      console.error("Error:", error);
      alert("Failed to add certificate.");
    });
}
