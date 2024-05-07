#cloud-config
package_update: true
package_upgrade: true
packages:
  - apt-transport-https
  - ca-certificates
  - curl
  - gnupg-agent
  - software-properties-common
runcmd:
  - curl -fsSL https://download.docker.com/linux/ubuntu/gpg | apt-key add -
  - add-apt-repository "deb [arch=$(dpkg --print-architecture)] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable"
  - apt-get update -y
  - apt-get install -y docker-ce docker-ce-cli containerd.io
  - systemctl start docker
  - systemctl enable docker
  - docker pull bookuhagh/iqp-backend
  # Pull runners
  - docker pull exercism/csharp-test-runner
  - docker pull exercism/fsharp-test-runner
  - docker pull exercism/java-test-runner
  - docker run -d -p 80:80 -v /var/run/docker.sock:/var/run/docker.sock --env PostgresConnectionString="Server=${host};Port=${port};Database=${database};User Id=${user};Password=${password};Include Error Detail=True;IntegratedSecurity=True;" --env TestRunner__SamplesFolderPath="/samples" --env TestRunner__SolutionsFolderPath="/solutions" bookuhagh/iqp-backend
