document
  .getElementById("loginForm")
  .addEventListener("submit", function (event) {
    event.preventDefault();
    const email = document.getElementById("email").value;
    const password = document.getElementById("password").value;

    user = {
      Email: email,
      Password: password,
    };

    fetch("https://localhost:44358/api/Registration/login", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      credentials: "include",
      body: JSON.stringify(user),
    })
      .then((response) => {
        if (response.status === 400) {
          throw new Error("Email Or Password is incorrect");
        }
        return response.text();
      })
      .then((data) => {
        alert(data);
        localStorage.setItem("user", data);
        window.location.href = "main.html";
      })
      .catch((error) => {
        displayError(error.message);
      });
    function displayError(message) {
      const errorElement = document.getElementById("errorMessage");
      if (errorElement) {
        errorElement.textContent = message;
        errorElement.style.display = "block";
        errorElement.style.color = "red";
      } else {
        console.error("Error element not found to display error:", message);
      }
    }
  });
