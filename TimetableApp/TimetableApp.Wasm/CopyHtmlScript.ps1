$s = Get-ChildItem -Name -Directory bin\Release\net5.0\publish\package*;
Copy-Item .\package_index.html (".\bin\Release\net5.0\publish\" + $s + "\index.html")