document.addEventListener("DOMContentLoaded", function () {
  loadCertificates();
});
function loadCertificates() {
  const loggedInUser = localStorage.getItem("user");
  const certificateForDto = localStorage.getItem("certificate");
  
  const requestPayload = {
    User: JSON.parse(loggedInUser),
    Certificate: JSON.parse(certificateForDto),
    };
  console.log(requestPayload)

  fetch("https://localhost:44358/api/Certificates/getCertificates", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(requestPayload),
  })
    .then((response) => {
      if (!response.ok) {
        throw new Error("Network response was not ok");
      }
      return response.json();
    })
    .then((data) => {
      displayCertificates(data);
    })
    .catch((error) => {
      console.error("Error:", error);
    });
}

function displayCertificates(certificates) {
  const certificatesList = document.getElementById("certificatesList");
  certificatesList.innerHTML = "";

  if (certificates.length === 0) {
    certificatesList.innerHTML = "<p>No certificates found.</p>";
    return;
  }

  const table = document.createElement("table");
  table.setAttribute("border", "1");

  const thead = document.createElement("thead");
  const headerRow = document.createElement("tr");

  const headers = ["Certificate Name", "Creation Date", "Expiry Date"];
  headers.forEach((headerText) => {
    const th = document.createElement("th");
    th.textContent = headerText;
    headerRow.appendChild(th);
  });

  thead.appendChild(headerRow);
  table.appendChild(thead);

  const tbody = document.createElement("tbody");

  certificates.forEach((certificate) => {
    const row = document.createElement("tr");

    const nameCell = document.createElement("td");
    nameCell.textContent = certificate.certificateName;
    row.appendChild(nameCell);

    const creationDateCell = document.createElement("td");
    const creationDate = new Date(
      certificate.creationDate
    ).toLocaleDateString();
    creationDateCell.textContent = creationDate;
    row.appendChild(creationDateCell);

    const expiryDateCell = document.createElement("td");
    const expiryDate = new Date(certificate.expiryDate).toLocaleDateString();
    expiryDateCell.textContent = expiryDate;
    row.appendChild(expiryDateCell);

    tbody.appendChild(row);
  });

  table.appendChild(tbody);
  certificatesList.appendChild(table);
}
